using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IUserCardRepository : IGenericRepository<UserCard, (Guid, Guid)>
{
    Task<IEnumerable<User>> GetAssigneesByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);

    Task<bool> IsAssignedAsync(
        Guid cardId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
