using MediatR;

namespace WorkBoard.Application.Features.Subscriptions.Commands.UpgradeUserSubscription;

public record UpgradeUserSubscriptionCommand(
    Guid UserId,
    string StripeSubscriptionId) : IRequest;
