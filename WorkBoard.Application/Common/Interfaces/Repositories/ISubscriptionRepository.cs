using WorkBoard.Application.Common.Dtos.Subscriptions;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ISubscriptionRepository
{
    Task<EnforceFreePlanLimitsResult> EnforceFreePlanLimitsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
