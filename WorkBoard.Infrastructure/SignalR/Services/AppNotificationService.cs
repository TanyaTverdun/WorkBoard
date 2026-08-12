using Microsoft.AspNetCore.SignalR;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Infrastructure.Constants;
using WorkBoard.Infrastructure.SignalR.Hubs;

namespace WorkBoard.Infrastructure.SignalR.Services;

public class AppNotificationService : IAppNotificationService
{
    private readonly IHubContext<AppHub> _hubContext;

    public AppNotificationService(IHubContext<AppHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendSidebarBoardStatusChangedAsync(
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync(
            AppHubEvents.SidebarBoardChanged,
            cancellationToken);
    }

    public async Task NotifyUserWorkspacesChangedAsync(
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync(
            AppHubEvents.WorkspacesListUpdated, 
            cancellationToken);
    }
}
