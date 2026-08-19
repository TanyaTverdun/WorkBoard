using MediatR;
using Microsoft.Extensions.Logging;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Application.Common.Interfaces.Stripe;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Subscriptions.Commands.CancelCurrentSubscription;

public class CancelCurrentSubscriptionCommandHandler
    : IRequestHandler<CancelCurrentSubscriptionCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IStripeService _stripeService;
    private readonly IUserContext _userContext;
    private readonly IAppNotificationService _appNotificationService;
    private readonly IBoardNotificationService _boardNotificationService;
    private readonly ILogger<CancelCurrentSubscriptionCommandHandler> _logger;

    public CancelCurrentSubscriptionCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IStripeService stripeService,
        IUserContext userContext,
        IAppNotificationService appNotificationService,
        IBoardNotificationService boardNotificationService,
        ILogger<CancelCurrentSubscriptionCommandHandler> logger)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _stripeService = stripeService;
        _userContext = userContext;
        _appNotificationService = appNotificationService;
        _boardNotificationService = boardNotificationService;
        _logger = logger;
    }

    public async Task Handle(
        CancelCurrentSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;
        if (userId == null)
        {
            _logger.LogWarning(
                "Cancel subscription failed: User ID is null in context.");
            return;
        }

        using var uow = _unitOfWorkFactory.Create();

        var user = await uow.UserRepository.GetByIdAsync(
            userId.Value, 
            cancellationToken);

        if (user == null || 
            string.IsNullOrEmpty(user.StripeSubscriptionId))
        {
            _logger.LogWarning(
                "User {UserId} has no active Stripe subscription to cancel.", 
                userId.Value);
            return;
        }

        try
        {
            await _stripeService.CancelSubscriptionAsync(
                user.StripeSubscriptionId,
                cancellationToken);

            _logger.LogInformation(
                "Successfully cancelled Stripe subscription {SubId}",
                user.StripeSubscriptionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error while cancelling subscription {SubId}", 
                user.StripeSubscriptionId);
            throw;
        }

        await uow.UserRepository.UpdateSubscriptionAsync(
            userId.Value,
            SubscriptionTier.Free,
            null,
            cancellationToken);

        var limitsResult = await uow.SubscriptionRepository.EnforceFreePlanLimitsAsync(
            userId.Value,
            cancellationToken);

        uow.Commit();

        try
        {
            await _appNotificationService.NotifyUserWorkspacesChangedAsync(
                userId.Value, 
                cancellationToken);

            foreach (var boardId in limitsResult.DeletedBoardIds)
            {
                await _boardNotificationService.SendBoardDeletedAsync(
                    boardId, 
                    cancellationToken);
            }

            foreach (var sectionInfo in limitsResult.DeletedSections)
            {
                await _boardNotificationService.SendSectionDeletedAsync(
                    sectionInfo.BoardId,
                    sectionInfo.SectionId,
                    cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex, 
                "Failed to send SignalR notifications after downgrade.");
        }
    }
}
