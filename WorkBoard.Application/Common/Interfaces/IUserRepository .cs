using System.Data;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces;

public interface IUserRepository : IGenericRepository<User, Guid>
{
    Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> AddAsync(
        User user,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);
}
