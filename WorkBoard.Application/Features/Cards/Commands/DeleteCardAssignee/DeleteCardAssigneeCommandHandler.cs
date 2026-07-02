using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Application.Features.Cards.Commands.DeleteCardAssignee;

public class DeleteCardAssigneeCommandHandler 
    : IRequestHandler<DeleteCardAssigneeCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public DeleteCardAssigneeCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(
        DeleteCardAssigneeCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var card = await uow.CardRepository.GetByIdAsync(
            request.CardId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {request.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId,
            currentUserId,
            cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to modify this card.");
        }
        
        var isAssigned = await uow.UserCardRepository.IsAssignedAsync(
            request.CardId,
            request.TargetUserId,
            cancellationToken);

        if (!isAssigned)
        {
            throw new NotFoundException(
                $"User with ID {request.TargetUserId} is not assigned to the card.");
        }

        try
        {
            await uow.UserCardRepository.DeleteAssigneeAsync(
                request.CardId,
                request.TargetUserId,
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
