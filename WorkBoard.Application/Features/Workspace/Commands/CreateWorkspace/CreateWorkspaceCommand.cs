using MediatR;

namespace WorkBoard.Application.Features.Workspace.Commands.CreateWorkspace;

public class CreateWorkspaceCommand : IRequest<Guid>
{
    public required string Name { get; set; }
}
