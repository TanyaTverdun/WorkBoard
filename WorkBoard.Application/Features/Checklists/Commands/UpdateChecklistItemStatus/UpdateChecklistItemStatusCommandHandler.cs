using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklistItemStatus;

public class UpdateChecklistItemStatusCommandHandler
    : IRequestHandler<UpdateChecklistItemStatusCommand, ChecklistItemDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public UpdateChecklistItemStatusCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<ChecklistItemDto> Handle(
        UpdateChecklistItemStatusCommand request,
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

        checklistItem.IsDone = request.IsDone;
        checklistItem.UpdatedAt = DateTime.UtcNow;
        checklistItem.UpdatedBy = currentUserId;

        try
        {
            await uow.ChecklistItemRepository.UpdateAsync(
                checklistItem, 
                cancellationToken);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return _mapper.Map<ChecklistItemDto>(checklistItem);
    }
}
