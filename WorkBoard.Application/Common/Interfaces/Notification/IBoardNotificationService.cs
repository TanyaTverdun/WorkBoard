using WorkBoard.Application.Common.Dtos.Cards;
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

}
