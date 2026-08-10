using Microsoft.AspNetCore.SignalR;

namespace WorkBoard.Infrastructure.SignalR.Hubs;

public class WorkspaceHub : Hub
{
    public async Task JoinWorkspace(string workspaceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, workspaceId);
    }

    public async Task LeaveWorkspace(string workspaceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, workspaceId);
    }
}
