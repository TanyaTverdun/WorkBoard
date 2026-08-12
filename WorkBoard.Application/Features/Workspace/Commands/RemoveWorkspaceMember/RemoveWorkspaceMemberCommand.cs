using MediatR;

namespace WorkBoard.Application.Features.Workspace.Commands.RemoveWorkspaceMember;

public record RemoveWorkspaceMemberCommand(
    Guid WorkspaceId,
    Guid TargetUserId) : IRequest<Unit>;
