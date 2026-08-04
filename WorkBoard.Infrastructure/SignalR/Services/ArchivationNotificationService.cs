using Microsoft.AspNetCore.SignalR;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;
using WorkBoard.Infrastructure.Constants;
using WorkBoard.Infrastructure.SignalR.Hubs;

namespace WorkBoard.Infrastructure.SignalR.Services;

public class ArchivationNotificationService : IArchivationNotificationService
{
    private readonly IHubContext<ArchivationHub> _hubContext;

    public ArchivationNotificationService(IHubContext<ArchivationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendArchivationStatusChangedAsync(
        Guid boardId,
        BoardArchiveStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.SendAsync(
            ArchivationHubEvents.ArchivationStatusChanged,
            boardId,
            newStatus,
            cancellationToken);
    }
}
