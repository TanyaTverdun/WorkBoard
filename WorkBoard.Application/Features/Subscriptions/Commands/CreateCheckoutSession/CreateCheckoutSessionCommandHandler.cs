using MediatR;
using WorkBoard.Application.Common.Dtos.Subscriptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Stripe;

namespace WorkBoard.Application.Features.Subscriptions.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionCommandHandler 
    : IRequestHandler<CreateCheckoutSessionCommand, CheckoutSessionResponseDto>
{
    private readonly IStripeService _stripeService;
    private readonly IUserContext _userContext;

    public CreateCheckoutSessionCommandHandler(
        IStripeService stripeService,
        IUserContext userContext)
    {
        _stripeService = stripeService;
        _userContext = userContext;
    }

    public async Task<CheckoutSessionResponseDto> Handle(
        CreateCheckoutSessionCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId?.ToString();

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        return await _stripeService.CreateCheckoutSessionAsync(
            userId, 
            cancellationToken);
    }
}
