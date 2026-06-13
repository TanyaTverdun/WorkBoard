using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User, Guid>
{
     Task<User?> GetByIdOrEmailAsync(
        Guid id,
        string? email,
        CancellationToken cancellationToken = default);
}
