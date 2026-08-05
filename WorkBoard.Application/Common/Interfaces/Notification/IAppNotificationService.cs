namespace WorkBoard.Application.Common.Interfaces.Notification;

public interface IAppNotificationService
{
    Task SendSidebarBoardStatusChangedAsync(
        CancellationToken cancellationToken = default);
}
