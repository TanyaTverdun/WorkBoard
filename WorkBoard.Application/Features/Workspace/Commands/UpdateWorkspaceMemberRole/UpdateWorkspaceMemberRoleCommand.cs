using MediatR;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Workspace.Commands.UpdateWorkspaceMemberRole;

public record UpdateWorkspaceMemberRoleCommand(
    Guid WorkspaceId,
    Guid TargetUserId,
    WorkspaceRole NewRole) : IRequest<Unit>;
