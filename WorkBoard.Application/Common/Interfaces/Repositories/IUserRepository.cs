using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User, Guid>
{
     Task<User?> GetByIdOrEmailAsync(
        Guid id,
        string? email,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        string searchTerm,
        CancellationToken cancellationToken = default);
}
