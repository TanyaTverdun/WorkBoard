using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDueDate;

public class UpdateCardDueDateCommandHandler 
    : IRequestHandler<UpdateCardDueDateCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public UpdateCardDueDateCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task Handle(
        UpdateCardDueDateCommand request,
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

        card.DueDate = request.DueDate;
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
    }
}
