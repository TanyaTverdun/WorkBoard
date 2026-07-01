using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class LabelRepository : GenericRepository<Label, Guid>, ILabelRepository
{
    public LabelRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal LabelRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<bool> IsNameUniqueAsync(
        Guid boardId,
        string name,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM Labels 
                    WHERE BoardId = @BoardId AND Name = @Name
                ) THEN 0 
                ELSE 1 
            END;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                Name = name
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<IReadOnlyList<Label>> GetByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                LabelId AS Id, 
                BoardId, 
                Name, 
                Color,
                CreatedAt,
                CreatedBy,
                UpdatedAt,
                UpdatedBy
            FROM 
                Labels 
            WHERE 
                BoardId = @BoardId
            ORDER BY 
                CreatedAt ASC;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var labels = await _connection.QueryAsync<Label>(command);

        return labels.ToList().AsReadOnly();
    }

    public async Task<IReadOnlyList<Label>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                l.LabelId AS Id, 
                l.BoardId, 
                l.Name, 
                l.Color,
                l.CreatedAt,
                l.CreatedBy,
                l.UpdatedAt,
                l.UpdatedBy
            FROM 
                Labels l
            INNER JOIN 
                CardLabels cl ON l.LabelId = cl.LabelId
            WHERE 
                cl.CardId = @CardId
            ORDER BY 
                l.CreatedAt ASC;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var labels = await _connection.QueryAsync<Label>(command);

        return labels.ToList().AsReadOnly();
    }
}
