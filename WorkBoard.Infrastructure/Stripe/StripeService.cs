using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using WorkBoard.Application.Common.Dtos.Subscriptions;
using WorkBoard.Application.Common.Interfaces.Stripe;
using WorkBoard.Infrastructure.Constants;
using WorkBoard.Infrastructure.Options;

namespace WorkBoard.Infrastructure.Stripe;

public class StripeService : IStripeService
{
    private readonly StripeOptions _stripeOptions;

    public StripeService(IOptions<StripeOptions> options)
    {
        _stripeOptions = options.Value;
    }

    public async Task<CheckoutSessionResponseDto> CreateCheckoutSessionAsync(
        string userId, 
        CancellationToken cancellationToken)
    {
        var options = new SessionCreateOptions
        {
            Mode = StripeConstants.SubscriptionMode,
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = _stripeOptions.ProPlanPriceId,
                    Quantity = 1,
                },
            },

            SuccessUrl = $"{_stripeOptions.ClientUrl}{StripeConstants.ClientRoutes.SubscriptionSuccess}",
            CancelUrl = $"{_stripeOptions.ClientUrl}{StripeConstants.ClientRoutes.SubscriptionCancel}",

            ClientReferenceId = userId
        };

        var service = new SessionService();
        Session session = await service.CreateAsync(
            options, 
            cancellationToken: cancellationToken);

        return new CheckoutSessionResponseDto
        {
            Url = session.Url
        };
    }

    public async Task CancelSubscriptionAsync(
        string stripeSubscriptionId,
        CancellationToken cancellationToken)
    {
        var service = new SubscriptionService();

        await service.CancelAsync(
            stripeSubscriptionId,
            cancellationToken: cancellationToken);
    }
}
