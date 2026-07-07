using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Application.Features.Checklists.Commands.DeleteChecklistItem;

public class DeleteChecklistItemCommandHandler 
    : IRequestHandler<DeleteChecklistItemCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public DeleteChecklistItemCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task Handle(
        DeleteChecklistItemCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var checklistItem = await uow.ChecklistItemRepository.GetByIdAsync(
            request.ItemId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Checklist item with ID {request.ItemId} was not found.");

        var checklist = await uow.ChecklistRepository.GetByIdAsync(
            checklistItem.ChecklistId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Checklist with ID {checklistItem.ChecklistId} was not found.");

        var card = await uow.CardRepository.GetByIdAsync(
            checklist.CardId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {checklist.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId, currentUserId, cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to modify items in this checklist.");
        }

        try
        {
            await uow.ChecklistItemRepository.DeleteAsync(
                checklistItem.Id, 
                cancellationToken);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}
