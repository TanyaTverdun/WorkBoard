using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

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

    public async Task AddMemberAsync(
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

        await _connection.ExecuteAsync(command);
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
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<bool>(command);
    }
}
