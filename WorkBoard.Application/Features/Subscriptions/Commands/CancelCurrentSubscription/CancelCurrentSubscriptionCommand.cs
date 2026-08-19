using MediatR;

namespace WorkBoard.Application.Features.Subscriptions.Commands.CancelCurrentSubscription;

public record CancelCurrentSubscriptionCommand() : IRequest;