using WorkBoard.Application.Common.Dtos.Subscriptions;

namespace WorkBoard.Application.Common.Interfaces.Stripe;

public interface IStripeService
{
    Task<CheckoutSessionResponseDto> CreateCheckoutSessionAsync(
        string userId, 
        CancellationToken cancellationToken);

    Task CancelSubscriptionAsync(
        string stripeSubscriptionId,
        CancellationToken cancellationToken);
}
