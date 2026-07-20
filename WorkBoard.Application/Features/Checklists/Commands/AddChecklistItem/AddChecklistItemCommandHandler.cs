using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Checklists.Commands.AddChecklistItem;

public class AddChecklistItemCommandHandler
    : IRequestHandler<AddChecklistItemCommand, ChecklistItemDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public AddChecklistItemCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBoardNotificationService notificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _notificationService = notificationService;
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

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUserId,
            Text = ActivityLogMessages.AddedChecklistItem(request.Title),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.ChecklistItemRepository.CreateAsync(
                checklistItem, 
                cancellationToken);

            await uow.ActivityLogRepository.CreateAsync(
                log,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        var logDto = _mapper.Map<ActivityLogDto>(log);
        logDto.FullName = _userContext.FullName!;
        logDto.Initials = InitialGenerator.Generate(_userContext.FullName!);

        await _notificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);

        var checklistItemDto = _mapper.Map<ChecklistItemDto>(checklistItem);

        var checklistItemAddedDto = new ChecklistItemAddedDto(
            card.Id,
            request.ChecklistId, 
            checklistItemDto);

        await _notificationService.SendChecklistItemAddedAsync(
            section.BoardId,
            checklistItemAddedDto,
            cancellationToken);

        return checklistItemDto;
    }
}
