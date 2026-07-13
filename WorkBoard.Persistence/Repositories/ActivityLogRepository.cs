using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class ActivityLogRepository
    : GenericRepository<ActivityLog, Guid>, IActivityLogRepository
{
    public ActivityLogRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal ActivityLogRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<ActivityLog>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                al.ActivityLogId AS Id,
                al.CardId,
                al.UserId,
                al.Text,
                al.CreatedAt,
                u.FullName
            FROM 
                ActivityLogs al
            INNER JOIN 
                Users u 
                ON al.UserId = u.Id
            WHERE 
                al.CardId = @CardId
            ORDER BY 
                al.CreatedAt DESC;";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                CardId = cardId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var logs = await _connection.QueryAsync<ActivityLog>(command);

        return logs.ToList().AsReadOnly();
    }
}
