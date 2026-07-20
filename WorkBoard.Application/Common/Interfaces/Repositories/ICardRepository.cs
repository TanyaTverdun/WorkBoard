using WorkBoard.Application.Common.Models;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ICardRepository : IGenericRepository<Card, Guid>
{
    Task<IReadOnlyList<Card>> GetCardsByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task<CardFullData?> GetCardFullDataAsync(
        Guid cardId,
        CancellationToken cancellationToken);

    Task UpdateCardPositionAsync(
        Guid cardId,
        Guid sectionId,
        double position,
        CancellationToken cancellationToken = default);
}
