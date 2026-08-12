using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Interfaces.Notification;

public interface IWorkspaceNotificationService
{
    Task SendMemberRoleUpdatedAsync(
        Guid workspaceId,
        Guid userId,
        WorkspaceRole newRole,
        CancellationToken cancellationToken = default);

    Task SendMemberAddedAsync(
        Guid workspaceId,
        WorkspaceMemberAddedDto payload,
        CancellationToken cancellationToken = default);

    Task SendMemberRemovedAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
