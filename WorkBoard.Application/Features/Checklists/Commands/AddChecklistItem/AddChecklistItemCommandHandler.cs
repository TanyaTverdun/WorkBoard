using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Checklists.Commands.AddChecklistItem;

public class AddChecklistItemCommandHandler
    : IRequestHandler<AddChecklistItemCommand, ChecklistItemDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public AddChecklistItemCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<ChecklistItemDto> Handle(
        AddChecklistItemCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var checklist = await uow.ChecklistRepository.GetByIdAsync(
            request.ChecklistId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Checklist with ID {request.ChecklistId} was not found.");

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
                "You do not have access to modify this checklist.");
        }

        var existingItems = await uow.ChecklistItemRepository.GetByChecklistIdAsync(
            request.ChecklistId, cancellationToken);

        if (existingItems.Any(x => x.Title.Equals(request.Title, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateTitleException(
                $"Item with title '{request.Title}' already exists in this checklist.");
        }

        var checklistItem = new ChecklistItem
        {
            Id = Guid.NewGuid(),
            ChecklistId = request.ChecklistId,
            Title = request.Title,
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        try
        {
            await uow.ChecklistItemRepository.CreateAsync(
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
