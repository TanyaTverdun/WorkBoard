using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class CardLabelRepository 
    : GenericRepository<CardLabel, (Guid, Guid)>, ICardLabelRepository
{
    public CardLabelRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal CardLabelRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task AddAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO CardLabels (
                CardId, 
                LabelId)
            VALUES (
                @CardId, 
                @LabelId);";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId,
                LabelId = labelId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }

    public async Task<bool> HasLabelAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM CardLabels 
                    WHERE CardId = @CardId AND LabelId = @LabelId
                ) THEN 1 
                ELSE 0 
            END;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId,
                LabelId = labelId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task RemoveAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM 
                CardLabels 
            WHERE 
                CardId = @CardId 
                AND LabelId = @LabelId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId,
                LabelId = labelId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }

    public async Task RemoveAllByLabelIdAsync(
        Guid labelId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            DELETE FROM 
                CardLabels 
            WHERE 
                LabelId = @LabelId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                LabelId = labelId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }
}
