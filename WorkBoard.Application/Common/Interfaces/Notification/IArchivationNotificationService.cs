using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Interfaces.Notification;

public interface IArchivationNotificationService
{
    Task SendArchivationStatusChangedAsync(
        Guid boardId,
        BoardArchiveStatus newStatus,
        CancellationToken cancellationToken = default);
}
