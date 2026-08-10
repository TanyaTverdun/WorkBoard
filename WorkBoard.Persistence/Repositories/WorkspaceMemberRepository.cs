using Dapper;
using System.Data;
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
                b.Id, 
                @UserId, 
                @UserRole
            FROM
                Boards b
            WHERE
                b.WorkspaceId = @WorkspaceId;

            SELECT 
                Id 
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
                Boards b ON bm.BoardId = b.Id
            WHERE 
                b.WorkspaceId = @WorkspaceId AND 
                bm.UserId = @UserId;

            INSERT INTO BoardMembers (
                BoardId, 
                UserId, 
                UserRole
            )
            SELECT 
                b.Id, 
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
                        bm.BoardId = b.Id AND 
                        bm.UserId = @UserId
                );

            SELECT 
                Id 
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
                ON bm.BoardId = b.Id
            WHERE 
                b.WorkspaceId = @WorkspaceId AND 
                bm.UserId = @UserId;

            DELETE FROM 
                WorkspaceMembers
            WHERE 
                WorkspaceId = @WorkspaceId AND 
                UserId = @UserId;

            SELECT 
                Id 
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
}
