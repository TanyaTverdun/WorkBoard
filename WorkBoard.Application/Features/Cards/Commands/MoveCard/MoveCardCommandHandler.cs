using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Cards.Commands.MoveCard;

public class MoveCardCommandHandler : IRequestHandler<MoveCardCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;

    public MoveCardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService boardNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _boardNotificationService = boardNotificationService;
    }

    public async Task<Unit> Handle(
        MoveCardCommand request, 
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            request.BoardId,
            cancellationToken);

        if (membership == null || 
            membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to move cards on this board.");
        }

        var card = await uow.CardRepository.GetByIdAsync(
            request.CardId, 
            cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(
                $"Card with ID {request.CardId} was not found.");
        }

        var targetSection = await uow.SectionRepository.GetByIdAsync(request.NewSectionId, cancellationToken);
        if (targetSection == null || 
            targetSection.BoardId != request.BoardId)
        {
            throw new NotFoundException(
                $"Section with ID {request.NewSectionId} was not found on this board.");
        }

        try
        {
            await uow.CardRepository.UpdateCardPositionAsync(
                request.CardId,
                request.NewSectionId,
                request.NewPosition,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        await _boardNotificationService.SendCardMovedAsync(
            request.BoardId,
            request.CardId,
            request.NewSectionId,
            request.NewPosition,
            cancellationToken);

        return Unit.Value;
    }
}
