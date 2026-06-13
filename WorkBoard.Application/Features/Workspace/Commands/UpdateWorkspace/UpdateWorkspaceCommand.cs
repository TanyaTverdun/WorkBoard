using MediatR;

namespace WorkBoard.Application.Features.Workspace.Commands.UpdateWorkspace;

public class UpdateWorkspaceCommand : IRequest<Unit>
{
    public required Guid WorkspaceId { get; set; }
    public required string Name { get; set; }
}
