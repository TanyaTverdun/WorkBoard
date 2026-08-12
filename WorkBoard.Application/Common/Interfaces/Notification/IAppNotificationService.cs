namespace WorkBoard.Application.Common.Interfaces.Notification;

public interface IAppNotificationService
{
    Task SendSidebarBoardStatusChangedAsync(
        CancellationToken cancellationToken = default);

    Task NotifyUserWorkspacesChangedAsync(
        Guid targetUserId,
        CancellationToken cancellationToken = default);
}
