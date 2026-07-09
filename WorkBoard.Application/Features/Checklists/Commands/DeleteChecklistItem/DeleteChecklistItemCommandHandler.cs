using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Checklists.Commands.DeleteChecklistItem;

public class DeleteChecklistItemCommandHandler 
    : IRequestHandler<DeleteChecklistItemCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _notificationService;
    private readonly IMapper _mapper;


    public DeleteChecklistItemCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService notificationService,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _notificationService = notificationService;
        _mapper = mapper;
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

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUserId,
            Text = ActivityLogMessages.DeletedChecklistItem(checklistItem.Title),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.ChecklistItemRepository.DeleteAsync(
                checklistItem.Id, 
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
    }
}
