using Microsoft.AspNetCore.SignalR;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Application.Common.Interfaces.Notification;
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
            .SendAsync("CardCreated", card, cancellationToken);
    }

    public async Task SendSectionCreatedAsync(
        Guid boardId, 
        SectionDto section, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync("SectionCreated", section, cancellationToken);
    }
}
