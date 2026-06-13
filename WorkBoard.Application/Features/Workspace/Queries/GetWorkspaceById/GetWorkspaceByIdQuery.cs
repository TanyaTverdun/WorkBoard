using MediatR;
using WorkBoard.Application.Common.Dtos.Workspaces;

namespace WorkBoard.Application.Features.Workspace.Queries.GetWorkspaceById;

public class GetWorkspaceByIdQuery : IRequest<UserWorkspaceDto>
{
    public required Guid WorkspaceId { get; set; }
}
