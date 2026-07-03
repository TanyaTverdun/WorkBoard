using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class ChecklistRepository
: GenericRepository<Checklist, Guid>, IChecklistRepository
{
    public ChecklistRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal ChecklistRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<Checklist?> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                ChecklistId AS Id,
                CardId,
                Name,
                CreatedAt,
                CreatedBy,
                UpdatedAt,
                UpdatedBy
            FROM 
                Checklists
            WHERE 
                CardId = @CardId;";

        var command = new CommandDefinition(
            sql,
            new { CardId = cardId },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<Checklist>(command);
    }
}
