using MediatR;
using Microsoft.Extensions.Logging;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Subscriptions.Commands.CancelUserSubscription;

public class CancelUserSubscriptionCommandHandler 
    : IRequestHandler<CancelUserSubscriptionCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<CancelUserSubscriptionCommandHandler> _logger;

    public CancelUserSubscriptionCommandHandler(
        IUserRepository userRepository,
        IUnitOfWorkFactory unitOfWorkFactory,
        ILogger<CancelUserSubscriptionCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;
    }

    public async Task Handle(
        CancelUserSubscriptionCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByStripeSubscriptionIdAsync(
            request.StripeSubscriptionId, 
            cancellationToken);

        if (user == null)
        {
            _logger.LogWarning(
                "Received cancellation for Stripe ID {StripeId}, but user not found.", 
                request.StripeSubscriptionId);
            return;
        }

        using var uow = _unitOfWorkFactory.Create();

        try
        {
            await uow.UserRepository.UpdateSubscriptionAsync(
                user.Id,
                SubscriptionTier.Free,
                null,
                cancellationToken);

            uow.Commit();

            _logger.LogInformation(
                "Subscription cancelled for User {UserId}", 
                user.Id);
        }
        catch (Exception ex)
        {
            uow.Rollback();
            _logger.LogError(
                ex, 
                "Failed to cancel subscription for User {UserId}", 
                user.Id);
            throw;
        }
    }
}
