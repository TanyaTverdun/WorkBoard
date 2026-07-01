using Microsoft.AspNetCore.SignalR;
using WorkBoard.Application.Common.Dtos.Cards;
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
            .SendAsync(BoardHubEvents.CardCreated, card, cancellationToken);
    }

    public async Task SendSectionCreatedAsync(
        Guid boardId, 
        SectionDto section, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(BoardHubEvents.SectionCreated, section, cancellationToken);
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
            .SendAsync(BoardHubEvents.SectionDeleted, sectionId, cancellationToken);
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
            .SendAsync(BoardHubEvents.MemberRemoved, userId, cancellationToken);
    }

    public async Task SendCardMovedAsync(
        Guid boardId,
        Guid cardId,
        Guid newSectionId,
        double newPosition,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(
                BoardHubEvents.CardMoved, 
                cardId, 
                newSectionId, 
                newPosition, 
                cancellationToken);
    }

    public async Task SendCardDeletedAsync(
        Guid boardId,
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(BoardHubEvents.CardDeleted, cardId, cancellationToken);
    }

    public async Task SendCardRenamedAsync(
        Guid boardId, 
        CardRenameDto data, 
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(boardId.ToString())
            .SendAsync(BoardHubEvents.CardRenamed, data, cancellationToken);
    }
}
