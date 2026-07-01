using MediatR;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardTitle;

public class UpdateCardTitleCommandHandler 
    : IRequestHandler<UpdateCardTitleCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;

    public UpdateCardTitleCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService boardNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _boardNotificationService = boardNotificationService;
    }

    public async Task<Unit> Handle(
        UpdateCardTitleCommand request,
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

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to edit cards on this board.");
        }

        var card = await uow.CardRepository.GetByIdAsync(
            request.CardId, 
            cancellationToken)
            ?? throw new NotFoundException(
                $"Card with ID {request.CardId} was not found.");

        card.Title = request.Title;
        card.UpdatedAt = DateTime.UtcNow;
        card.UpdatedBy = currentUserId;

        try
        {
            await uow.CardRepository.UpdateAsync(card, cancellationToken);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        var cardRenameDto = new CardRenameDto(
            card.Id,
            card.Title
        );

        await _boardNotificationService.SendCardRenamedAsync(
            request.BoardId,
            cardRenameDto,
            cancellationToken);

        return Unit.Value;
    }
}
