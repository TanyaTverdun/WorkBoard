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

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommandHandler
    : IRequestHandler<UpdateChecklistItemCommand, ChecklistItemDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public UpdateChecklistItemCommandHandler(
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
        UpdateChecklistItemCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = await _userContext.GetCurrentUserFullProfileAsync(
            cancellationToken)
            ?? throw new UnauthorizedAccessException(
                "User profile not found in database.");

        using var uow = _unitOfWorkFactory.Create();

        var checklistItem = await uow.ChecklistItemRepository.GetByIdAsync(
            request.ItemId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Checklist item with ID {request.ItemId} was not found.");

        var checklist = await uow.ChecklistRepository.GetByIdAsync(
            checklistItem.ChecklistId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Checklist with ID {checklistItem.ChecklistId} was not found.");

        var card = await uow.CardRepository.GetByIdAsync(
            checklist.CardId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {checklist.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId, 
            currentUser.Id, 
            cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to modify items in this checklist.");
        }

        var existingItems = await uow.ChecklistItemRepository.GetByChecklistIdAsync(
            checklistItem.ChecklistId, 
            cancellationToken);

        if (existingItems.Any(x => x.Id != request.ItemId &&
                                   x.Title.Equals(
                                       request.Title, 
                                       StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateTitleException(
                $"Checklist item with title '{request.Title}' already exists.");
        }

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUser.Id,
            Text = ActivityLogMessages.RenamedChecklistItem(
                checklist.Name, 
                request.Title),
            CreatedAt = DateTime.UtcNow
        };

        checklistItem.Title = request.Title;
        checklistItem.UpdatedAt = DateTime.UtcNow;
        checklistItem.UpdatedBy = currentUser.Id;

        try
        {
            await uow.ChecklistItemRepository.UpdateAsync(
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
        logDto.FullName = currentUser.FullName!;
        logDto.Initials = InitialGenerator.Generate(currentUser.FullName);
        logDto.AvatarUrl = currentUser.AvatarUrl;
        logDto.AvatarColor = currentUser.AvatarColor;

        await _notificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);

        var checklistItemRenamedDto = new ChecklistItemRenamedDto(
            checklistItem.ChecklistId,
            checklistItem.Id,
            checklistItem.Title);

        await _notificationService.SendChecklistItemRenamedAsync(
            section.BoardId,
            checklistItemRenamedDto,
            cancellationToken);

        return _mapper.Map<ChecklistItemDto>(checklistItem);
    }
}
