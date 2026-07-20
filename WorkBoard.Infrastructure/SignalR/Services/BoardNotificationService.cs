using Microsoft.AspNetCore.SignalR;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Attachments;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Dtos.Comments;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Application.Common.Dtos.Sections;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;
using WorkBoard.Infrastructure.Constants;
using WorkBoard.Infrastructure.SignalR.Hubs;

namespace WorkBoard.Infrastructure.SignalR.Services;

public class BoardNotificationService : IBoardNotificationService
{
    private readonly IHubContext<BoardHub> _hubContext;

    public BoardNotificationService(IHubContext<BoardHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendCardCreatedAsync(
        Guid boardId,
        CardDto card, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CardCreated, 
                card, 
                cancellationToken);
    }

    public async Task SendSectionCreatedAsync(
        Guid boardId, 
        SectionDto section, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.SectionCreated, 
                section, 
                cancellationToken);
    }

    public async Task SendSectionRenamedAsync(
        Guid boardId,
        SectionRenameDto section,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.SectionRenamed,
                section,
                cancellationToken);
    }

    public async Task SendSectionDeletedAsync(
        Guid boardId, 
        Guid sectionId, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.SectionDeleted, 
                sectionId, 
                cancellationToken);
    }

    public async Task SendSectionMovedAsync(
        Guid boardId, 
        Guid sectionId, 
        double newPosition, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
            BoardHubEvents.SectionMoved, 
            new { 
                SectionId = sectionId, 
                NewPosition = newPosition 
            }, 
            cancellationToken);
    }

    public async Task SendMemberRoleUpdatedAsync(
        Guid boardId, 
        Guid userId, 
        BoardRole newRole,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
            BoardHubEvents.MemberRoleUpdated, 
            new 
            { 
                UserId = userId, 
                NewRole = newRole 
            }, 
            cancellationToken);
    }

    public async Task SendMemberRemovedAsync(
        Guid boardId, 
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.MemberRemoved, 
                userId, 
                cancellationToken);
    }

    public async Task SendCardDeletedAsync(
        Guid boardId,
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CardDeleted, 
                cardId, 
                cancellationToken);
    }

    public async Task SendCardRenamedAsync(
        Guid boardId, 
        CardRenameDto data, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CardRenamed, 
                data, 
                cancellationToken);
    }

    public async Task SendCommentAddedAsync(
        Guid boardId,
        CommentDto comment,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CommentAdded, 
                comment, 
                cancellationToken);
    }

    public async Task SendActivityLogAddedAsync(
        Guid boardId,
        ActivityLogDto log,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ActivityLogAdded, 
                log, 
                cancellationToken);
    }

    public async Task SendCardDueDateUpdatedAsync(
        Guid boardId,
        CardDueDateUpdateDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CardDueDateUpdated, 
                data, 
                cancellationToken);
    }

    public async Task SendLabelAddedToCardAsync(
        Guid boardId, 
        Guid cardId, 
        LabelDto label, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.LabelAddedToCard, 
                new 
                { 
                    CardId = cardId, 
                    Label = label 
                }, 
                cancellationToken);
    }

    public async Task SendLabelRemovedFromCardAsync(
        Guid boardId, 
        Guid cardId, 
        Guid labelId, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.LabelRemovedFromCard, 
                new 
                { 
                    CardId = cardId, 
                    LabelId = labelId 
                }, 
                cancellationToken);
    }

    public async Task SendLabelCreatedAsync(
        Guid boardId, 
        LabelDto label, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.LabelCreated, 
                label, 
                cancellationToken);
    }

    public async Task SendLabelUpdatedAsync(
        Guid boardId, 
        LabelDto label, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.LabelUpdated, 
                label, 
                cancellationToken);
    }

    public async Task SendLabelDeletedAsync(
        Guid boardId, 
        Guid labelId, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.LabelDeleted, 
                labelId, 
                cancellationToken);
    }

    public async Task SendAssigneeAddedAsync(
        Guid boardId,
        AssigneeAddDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.AssigneeAdded, 
                data, 
                cancellationToken);
    }

    public async Task SendAssigneeRemovedAsync(
        Guid boardId,
        AssigneeRemoveDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.AssigneeRemoved, 
                data, 
                cancellationToken);
    }

    public async Task SendCardMovedAsync(
        Guid boardId,
        CardMovedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CardMoved, 
                data, 
                cancellationToken);
    }

    public async Task SendCardDescriptionUpdatedAsync(
        Guid boardId,
        CardDescriptionUpdateDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CardDescriptionUpdated, 
                data, 
                cancellationToken);
    }

    public async Task SendChecklistItemAddedAsync(
        Guid boardId, 
        ChecklistItemAddedDto data, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ChecklistItemAdded, 
                data, 
                cancellationToken);
    }

    public async Task SendChecklistCreatedAsync(
        Guid boardId,
        ChecklistCreatedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ChecklistCreated, 
                data, 
                cancellationToken);
    }

    public async Task SendChecklistDeletedAsync(
        Guid boardId,
        ChecklistDeletedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ChecklistDeleted, 
                data, 
                cancellationToken);
    }

    public async Task SendChecklistItemDeletedAsync(
        Guid boardId,
        ChecklistItemDeletedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ChecklistItemDeleted, 
                data, 
                cancellationToken);
    }

    public async Task SendChecklistRenamedAsync(
        Guid boardId,
        ChecklistRenamedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ChecklistRenamed, 
                data, 
                cancellationToken);
    }

    public async Task SendChecklistItemRenamedAsync(
        Guid boardId,
        ChecklistItemRenamedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ChecklistItemRenamed, 
                data, 
                cancellationToken);
    }

    public async Task SendChecklistItemStatusUpdatedAsync(
        Guid boardId,
        ChecklistItemStatusUpdatedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.ChecklistItemStatusUpdated, 
                data, 
                cancellationToken);
    }

    public async Task SendAttachmentAddedAsync(
        Guid boardId,
        AttachmentAddedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.AttachmentAdded, 
                data, 
                cancellationToken);
    }

    public async Task SendAttachmentDeletedAsync(
        Guid boardId,
        AttachmentDeletedDto data,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.AttachmentDeleted, 
                data, 
                cancellationToken);
    }
}
