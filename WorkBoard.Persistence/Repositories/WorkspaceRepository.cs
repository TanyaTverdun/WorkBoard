using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class WorkspaceRepository : 
    GenericRepository<Workspace, Guid>, IWorkspaceRepository
{
    public WorkspaceRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal WorkspaceRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<UserWorkspaceDto>> GetByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                w.WorkspaceId AS Id,
                w.Name,
                w.SubscriptionTier,
                wm.UserRole
            FROM 
                Workspaces w
            JOIN 
                WorkspaceMembers wm 
                ON w.WorkspaceId = wm.WorkspaceId
            WHERE 
                wm.UserId = @UserId
            ORDER BY 
                w.CreatedAt DESC;";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                UserId = userId 
            },
            cancellationToken: cancellationToken);

        var workspaces = await _connection.QueryAsync<UserWorkspaceDto>(command);

        return workspaces.ToList().AsReadOnly();
    }
}
