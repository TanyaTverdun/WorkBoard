using MediatR;

namespace WorkBoard.Application.Features.Subscriptions.Commands.CancelUserSubscription;

public record CancelUserSubscriptionCommand(
    string StripeSubscriptionId) : IRequest;
