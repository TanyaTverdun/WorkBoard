using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IActivityLogRepository 
    : IGenericRepository<ActivityLog, Guid>
{
    Task<IReadOnlyList<ActivityLog>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}
