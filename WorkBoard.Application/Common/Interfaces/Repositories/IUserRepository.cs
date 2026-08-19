using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

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

    Task<int> UpdateAvatarColorAsync(
        Guid userId,
        string color,
        CancellationToken cancellationToken = default);

    Task<int> UpdateAvatarUrlAsync(
        Guid userId,
        string avatarUrl,
        CancellationToken cancellationToken = default);

    Task<User?> GetByStripeSubscriptionIdAsync(
        string stripeSubscriptionId,
        CancellationToken cancellationToken = default);

    Task<int> UpdateSubscriptionAsync(
        Guid userId,
        SubscriptionTier tier,
        string? stripeSubscriptionId,
        CancellationToken cancellationToken = default);
}
