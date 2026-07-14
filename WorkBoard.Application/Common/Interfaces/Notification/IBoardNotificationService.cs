using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Attachments;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Dtos.Comments;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Application.Common.Dtos.Sections;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Interfaces.Notification;

public interface IBoardNotificationService
{
    Task SendCardCreatedAsync(
        Guid boardId, 
        CardDto card, 
        CancellationToken cancellationToken = default);

    Task SendSectionCreatedAsync(
        Guid boardId, 
        SectionDto section, 
        CancellationToken cancellationToken = default);

    Task SendSectionRenamedAsync(
        Guid boardId, 
        SectionRenameDto section, 
        CancellationToken cancellationToken = default);

    Task SendSectionDeletedAsync(
        Guid boardId, 
        Guid sectionId, 
        CancellationToken cancellationToken = default);

    Task SendSectionMovedAsync(
        Guid boardId, 
        Guid sectionId, 
        double newPosition, 
        CancellationToken cancellationToken = default);

    Task SendMemberRoleUpdatedAsync(
        Guid boardId, 
        Guid userId, 
        BoardRole newRole,
        CancellationToken cancellationToken = default);

    Task SendMemberRemovedAsync(
        Guid boardId, 
        Guid userId, 
        CancellationToken cancellationToken = default);

    Task SendCardDeletedAsync(
        Guid boardId,
        Guid cardId,
        CancellationToken cancellationToken = default);

    Task SendCardRenamedAsync(
        Guid boardId,
        CardRenameDto data,
        CancellationToken cancellationToken = default);

    Task SendCommentAddedAsync(
        Guid boardId,
        CommentDto comment,
        CancellationToken cancellationToken = default);

    Task SendActivityLogAddedAsync(
        Guid boardId,
        ActivityLogDto log,
        CancellationToken cancellationToken = default);

    Task SendCardDueDateUpdatedAsync(
        Guid boardId,
        CardDueDateUpdateDto data,
        CancellationToken cancellationToken = default);

    Task SendLabelAddedToCardAsync(
        Guid boardId, 
        Guid cardId, 
        LabelDto label, 
        CancellationToken cancellationToken = default);

    Task SendLabelRemovedFromCardAsync(
        Guid boardId, 
        Guid cardId, 
        Guid labelId, 
        CancellationToken cancellationToken = default);

    Task SendLabelCreatedAsync(
        Guid boardId, 
        LabelDto label, 
        CancellationToken cancellationToken = default);

    Task SendLabelUpdatedAsync(
        Guid boardId, 
        LabelDto label, 
        CancellationToken cancellationToken = default);

    Task SendLabelDeletedAsync(
        Guid boardId, 
        Guid labelId, 
        CancellationToken cancellationToken = default);

    Task SendAssigneeAddedAsync(
        Guid boardId,
        AssigneeAddDto data,
        CancellationToken cancellationToken = default);

    Task SendAssigneeRemovedAsync(
        Guid boardId,
        AssigneeRemoveDto data,
        CancellationToken cancellationToken = default);

    Task SendCardMovedAsync(
        Guid boardId,
        CardMovedDto data,
        CancellationToken cancellationToken = default);

    Task SendCardDescriptionUpdatedAsync(
        Guid boardId,
        CardDescriptionUpdateDto data,
        CancellationToken cancellationToken = default);

    Task SendChecklistItemAddedAsync(
        Guid boardId, 
        ChecklistItemAddedDto data, 
        CancellationToken cancellationToken = default);

    Task SendChecklistCreatedAsync(
        Guid boardId,
        ChecklistCreatedDto data,
        CancellationToken cancellationToken = default);

    Task SendChecklistDeletedAsync(
        Guid boardId,
        ChecklistDeletedDto data,
        CancellationToken cancellationToken = default);

    Task SendChecklistItemDeletedAsync(
        Guid boardId,
        ChecklistItemDeletedDto data,
        CancellationToken cancellationToken = default);

    Task SendChecklistRenamedAsync(
        Guid boardId,
        ChecklistRenamedDto data,
        CancellationToken cancellationToken = default);

    Task SendChecklistItemRenamedAsync(
        Guid boardId,
        ChecklistItemRenamedDto data,
        CancellationToken cancellationToken = default);

    Task SendChecklistItemStatusUpdatedAsync(
        Guid boardId,
        ChecklistItemStatusUpdatedDto data,
        CancellationToken cancellationToken = default);

    Task SendAttachmentAddedAsync(
        Guid boardId,
        AttachmentAddedDto data,
        CancellationToken cancellationToken = default);

    Task SendAttachmentDeletedAsync(
        Guid boardId,
        AttachmentDeletedDto data,
        CancellationToken cancellationToken = default);
}
