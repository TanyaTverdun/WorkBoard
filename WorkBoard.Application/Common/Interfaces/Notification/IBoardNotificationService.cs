using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Dtos.Comments;
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

    Task SendCardMovedAsync(
        Guid boardId,
        Guid cardId,
        Guid newSectionId,
        double newPosition,
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
}
