using Microsoft.AspNetCore.SignalR;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;
using WorkBoard.Infrastructure.Constants;
using WorkBoard.Infrastructure.SignalR.Hubs;

namespace WorkBoard.Infrastructure.SignalR.Services;

public class WorkspaceNotificationService : IWorkspaceNotificationService
{
    private readonly IHubContext<WorkspaceHub> _hubContext;

    public WorkspaceNotificationService(IHubContext<WorkspaceHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMemberRoleUpdatedAsync(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole newRole,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(workspaceId.ToString())
            .SendAsync(
                WorkspaceHubEvents.MemberRoleUpdated,
                new
                {
                    UserId = userId,
                    NewRole = newRole
                },
                cancellationToken);
    }

    public async Task SendMemberAddedAsync(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole role,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(workspaceId.ToString())
            .SendAsync(
                WorkspaceHubEvents.MemberAdded,
                new
                {
                    UserId = userId,
                    Role = role
                },
                cancellationToken);
    }

    public async Task SendMemberRemovedAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.Group(workspaceId.ToString())
            .SendAsync(
                WorkspaceHubEvents.MemberRemoved,
                userId,
                cancellationToken);
    }
}
