using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class ChecklistItemRepository 
    : GenericRepository<ChecklistItem, Guid>, IChecklistItemRepository
{
    public ChecklistItemRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal ChecklistItemRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<ChecklistItem>> GetByChecklistIdAsync(
        Guid checklistId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                ChecklistItemId AS Id,
                ChecklistId,
                Title,
                IsDone,
                CreatedAt,
                CreatedBy,
                UpdatedAt,
                UpdatedBy
            FROM 
                Checklist_items
            WHERE 
                ChecklistId = @ChecklistId
            ORDER BY 
                CreatedAt ASC;";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                ChecklistId = checklistId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var items = await _connection.QueryAsync<ChecklistItem>(command);

        return items.ToList().AsReadOnly();
    }
}
