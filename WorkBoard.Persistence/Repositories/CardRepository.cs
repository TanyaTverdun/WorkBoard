using Dapper;
using System.Data;
using System.Text;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class CardRepository : GenericRepository<Card, Guid>, ICardRepository
{
    public CardRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal CardRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<Card>> GetCardsByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                c.CardId AS Id,
                c.SectionId,
                c.Title,
                c.Description,
                c.DueDate,
                c.Position,
                c.CreatedAt,
                c.CreatedBy,
                c.UpdatedAt,
                c.UpdatedBy
            FROM 
                Cards c
            JOIN 
                Sections s 
                ON c.SectionId = s.SectionId
            WHERE 
                s.BoardId = @BoardId
            ORDER BY 
                c.Position ASC;";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                BoardId = boardId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var cards = await _connection.QueryAsync<Card>(command);

        return cards.ToList().AsReadOnly();
    }

    public async Task UpdateCardPositionAsync(
        Guid cardId,
        Guid sectionId,
        double position,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE 
                Cards 
            SET 
                SectionId = @SectionId, 
                Position = @Position,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE 
                CardId = @CardId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId,
                SectionId = sectionId,
                Position = position
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }
}
