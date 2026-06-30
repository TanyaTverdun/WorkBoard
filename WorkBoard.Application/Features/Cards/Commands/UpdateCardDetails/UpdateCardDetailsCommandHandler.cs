using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDetails;

public class UpdateCardDetailsCommandHandler 
    : IRequestHandler<UpdateCardDetailsCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public UpdateCardDetailsCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService boardNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(
        UpdateCardDetailsCommand request,
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
            cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(
                $"Card with ID {request.CardId} was not found.");
        }

        try
        {
            await uow.CardRepository.UpdateCardDetailsAsync(
                request.CardId,
                request.Title,
                request.Description,
                request.IsDescriptionUpdated,
                currentUserId,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return Unit.Value;
    }
}
