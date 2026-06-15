using MediatR;

namespace WorkBoard.Application.Features.Workspace.Commands.DeleteWorkspace;

public class DeleteWorkspaceCommand : IRequest<Unit>
{
    public required Guid WorkspaceId { get; set; }
}
