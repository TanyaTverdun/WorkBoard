using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ICardLabelRepository : IGenericRepository<CardLabel, (Guid, Guid)>
{
    Task AddAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default);

    Task<bool> HasLabelAsync(
        Guid cardId,
        Guid labelId,
        CancellationToken cancellationToken = default);

    Task RemoveAllByLabelIdAsync(
        Guid labelId,
        CancellationToken cancellationToken = default);
}
