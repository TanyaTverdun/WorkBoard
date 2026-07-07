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
                c.ChecklistId AS Id,
                c.CardId,
                c.Name,
                c.CreatedAt,
                c.CreatedBy,
                c.UpdatedAt,
                c.UpdatedBy,
                ci.ChecklistItemId AS Id,
                ci.ChecklistId,
                ci.Title,
                ci.IsDone,
                ci.CreatedAt,
                ci.CreatedBy,
                ci.UpdatedAt,
                ci.UpdatedBy
            FROM 
                Checklists c
            LEFT JOIN 
                Checklist_items ci ON c.ChecklistId = ci.ChecklistId
            WHERE 
                c.CardId = @CardId
            ORDER BY 
                ci.CreatedAt ASC;";

        var checklistDictionary = new Dictionary<Guid, Checklist>();

        var command = new CommandDefinition(
            sql,
            new 
            { 
                CardId = cardId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.QueryAsync<Checklist, ChecklistItem, Checklist>(
            command,
            (checklist, item) =>
            {
                if (!checklistDictionary.TryGetValue(checklist.Id, out var currentChecklist))
                {
                    currentChecklist = checklist;
                    checklistDictionary.Add(currentChecklist.Id, currentChecklist);
                }

                if (item != null)
                {
                    currentChecklist.Items.Add(item);
                }

                return currentChecklist;
            },
            splitOn: "Id"
        );

        return checklistDictionary.Values.FirstOrDefault();
    }
}
