using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Persistence.Repositories;

public class WorkspaceMemberRepository : 
    GenericRepository<WorkspaceMember, (Guid, Guid)>, IWorkspaceMemberRepository
{
    public WorkspaceMemberRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal WorkspaceMemberRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IEnumerable<Guid>> AddMemberAsync(
        WorkspaceMember member, 
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO WorkspaceMembers (
                UserId, 
                WorkspaceId, 
                UserRole)
            VALUES (
                @UserId, 
                @WorkspaceId, 
                @UserRole);

            INSERT INTO BoardMembers(
                BoardId, 
                UserId, 
                UserRole)
            SELECT
                b.BoardId, 
                @UserId, 
                @UserRole
            FROM
                Boards b
            WHERE
                b.WorkspaceId = @WorkspaceId;

            SELECT 
                BoardId 
            FROM 
                Boards 
            WHERE 
                WorkspaceId = @WorkspaceId;";

        var command = new CommandDefinition(
            sql,
            member,
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<Guid>(command);
    }

    public async Task<IEnumerable<Guid>> AddMemberOnlyToWorkspaceAsync(
        WorkspaceMember member,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO WorkspaceMembers (
                UserId, 
                WorkspaceId, 
                UserRole)
            VALUES (
                @UserId, 
                @WorkspaceId, 
                @UserRole);";

        var command = new CommandDefinition(
            sql,
            member,
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<Guid>(command);
    }

    public async Task<bool> IsMemberAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM 
                        WorkspaceMembers 
                    WHERE 
                        WorkspaceId = @WorkspaceId AND 
                        UserId = @UserId
                ) THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
            END;";
         var command = new CommandDefinition(
            sql,
            new
            {
              WorkspaceId = workspaceId,
              UserId = userId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);
            
          return await _connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<WorkspaceMember?> GetMembershipAsync(
        Guid userId,
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                UserId, 
                WorkspaceId, 
                UserRole
            FROM 
                WorkspaceMembers
            WHERE 
                UserId = @UserId 
                AND WorkspaceId = @WorkspaceId;";
         var command = new CommandDefinition(
            sql,
            new
            {
              UserId = userId,
              WorkspaceId = workspaceId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);
         return await _connection.QueryFirstOrDefaultAsync<WorkspaceMember>(command);
    }

    public async Task<IEnumerable<Guid>> UpdateRoleAsync(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole newRole,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE 
                WorkspaceMembers 
            SET 
                UserRole = @NewRole
            WHERE 
                WorkspaceId = @WorkspaceId AND 
                UserId = @UserId;

            UPDATE bm
            SET 
                bm.UserRole = @NewRole
            FROM 
                BoardMembers bm
            INNER JOIN 
                Boards b 
                ON bm.BoardId = b.BoardId
            WHERE 
                b.WorkspaceId = @WorkspaceId AND 
                bm.UserId = @UserId;

            INSERT INTO BoardMembers (
                BoardId, 
                UserId, 
                UserRole
            )
            SELECT 
                b.BoardId, 
                @UserId, 
                @NewRole
            FROM 
                Boards b
            WHERE 
                b.WorkspaceId = @WorkspaceId
                AND NOT EXISTS (
                    SELECT 1 
                    FROM 
                        BoardMembers bm 
                    WHERE 
                        bm.BoardId = b.BoardId AND 
                        bm.UserId = @UserId
                );

            SELECT 
                BoardId 
            FROM 
                Boards 
            WHERE 
                WorkspaceId = @WorkspaceId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                WorkspaceId = workspaceId,
                UserId = userId,
                NewRole = (byte)newRole
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<Guid>(command);
    }

    public async Task<IEnumerable<Guid>> RemoveMemberAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE 
                bm
            FROM 
                BoardMembers bm
            INNER JOIN 
                Boards b 
                ON bm.BoardId = b.BoardId
            WHERE 
                b.WorkspaceId = @WorkspaceId AND 
                bm.UserId = @UserId;

            DELETE FROM 
                WorkspaceMembers
            WHERE 
                WorkspaceId = @WorkspaceId AND 
                UserId = @UserId;

            SELECT 
                BoardId 
            FROM 
                Boards 
            WHERE 
                WorkspaceId = @WorkspaceId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                WorkspaceId = workspaceId,
                UserId = userId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<Guid>(command);
    }

    public async Task<IReadOnlyList<WorkspaceMemberDto>> GetMembersByWorkspaceIdAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                u.UserId AS Id,
                u.FullName AS Name,
                u.Email,
                wm.UserRole AS Role,
                u.AvatarUrl,
                u.AvatarColor
            FROM 
                WorkspaceMembers wm
            INNER JOIN 
                Users u ON wm.UserId = u.UserId
            WHERE 
                wm.WorkspaceId = @WorkspaceId
            ORDER BY 
                wm.UserRole ASC,
                u.FullName ASC;";

        var command = new CommandDefinition(
            sql,
            new
            {
                WorkspaceId = workspaceId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var members = await _connection.QueryAsync<WorkspaceMemberDto>(command);

        return members.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<UserSearchDto>> SearchWorkspaceAssignableUsersAsync(
        Guid workspaceId,
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT TOP 10
                UserId,
                FullName,
                Email,
                AvatarUrl,
                AvatarColor
            FROM 
                Users
            WHERE 
                Email LIKE @SearchTerm + '%'
                AND UserId NOT IN (
                    SELECT 
                        UserId 
                    FROM 
                        WorkspaceMembers 
                    WHERE 
                        WorkspaceId = @WorkspaceId
                )
            ORDER BY 
                Email;";

        var command = new CommandDefinition(
            sql,
            new
            {
                WorkspaceId = workspaceId,
                SearchTerm = searchTerm
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var users = await _connection.QueryAsync<UserSearchDto>(command);

        return users.ToList().AsReadOnly();
    }

    public async Task<IEnumerable<Guid>> GetMemberUserIdsByOwnerAsync(
        Guid ownerId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT DISTINCT 
                wm.UserId
            FROM 
                WorkspaceMembers wm
            INNER JOIN 
                Workspaces w ON 
                wm.WorkspaceId = w.WorkspaceId
            WHERE 
                w.CreatedBy = @OwnerId;";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                OwnerId = ownerId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<Guid>(command);
    }
}
