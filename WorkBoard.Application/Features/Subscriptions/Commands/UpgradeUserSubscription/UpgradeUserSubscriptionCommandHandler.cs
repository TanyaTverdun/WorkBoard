using MediatR;
using Microsoft.Extensions.Logging;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Subscriptions.Commands.UpgradeUserSubscription;

public class UpgradeUserSubscriptionCommandHandler
    : IRequestHandler<UpgradeUserSubscriptionCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IAppNotificationService _appNotificationService;
    private readonly ILogger<UpgradeUserSubscriptionCommandHandler> _logger;

    public UpgradeUserSubscriptionCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IAppNotificationService appNotificationService,
        ILogger<UpgradeUserSubscriptionCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _appNotificationService = appNotificationService;
        _logger = logger;
    }

    public async Task Handle(
        UpgradeUserSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        using var uow = _unitOfWorkFactory.Create();

        List<Guid> usersToNotify = new();

        try
        {
            var affectedRows = await uow.UserRepository.UpdateSubscriptionAsync(
                request.UserId,
                SubscriptionTier.Pro,
                request.StripeSubscriptionId,
                cancellationToken);

            if (affectedRows == 0)
            {
                _logger.LogWarning(
                    "Webhook Upgrade: User with ID {UserId} was not found. " +
                    "Stripe Sub ID: {StripeId}",
                    request.UserId,
                    request.StripeSubscriptionId);

                return;
            }

            var memberIds = await uow.WorkspaceMemberRepository
                .GetMemberUserIdsByOwnerAsync(
                    request.UserId, 
                    cancellationToken);

            usersToNotify.AddRange(memberIds);

            if (!usersToNotify.Contains(request.UserId))
            {
                usersToNotify.Add(request.UserId);
            }

            uow.Commit();

            _logger.LogInformation(
                "Successfully upgraded user {UserId} to Pro tier. " +
                "Stripe Sub ID: {StripeId}",
                request.UserId,
                request.StripeSubscriptionId);
        }
        catch (Exception ex)
        {
            uow.Rollback();

            _logger.LogError(
                ex,
                "An error occurred while upgrading subscription " +
                "for user {UserId}",
                request.UserId);

            throw;
        }

        foreach (var userId in usersToNotify)
        {
            await _appNotificationService.NotifyUserWorkspacesChangedAsync(
                userId,
                cancellationToken);
        }
    }
}
