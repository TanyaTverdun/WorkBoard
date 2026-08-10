using MediatR;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Workspace.Commands.AddWorkspaceMember;

public record AddWorkspaceMemberCommand(
    Guid WorkspaceId,
    Guid TargetUserId,
    WorkspaceRole Role) : IRequest<Unit>;
