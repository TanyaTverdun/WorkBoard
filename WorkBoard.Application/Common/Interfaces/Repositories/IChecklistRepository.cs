using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IChecklistRepository : IGenericRepository<Checklist, Guid>
{
    Task<IEnumerable<Checklist>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}
