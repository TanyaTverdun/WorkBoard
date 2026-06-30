using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ICardRepository : IGenericRepository<Card, Guid>
{
    Task<IReadOnlyList<Card>> GetCardsByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task UpdateCardPositionAsync(
        Guid cardId,
        Guid sectionId,
        double position,
        CancellationToken cancellationToken = default);
}
