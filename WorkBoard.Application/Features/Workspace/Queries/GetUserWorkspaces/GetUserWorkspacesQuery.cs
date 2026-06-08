using MediatR;
using WorkBoard.Application.Common.Dtos.Workspaces;

namespace WorkBoard.Application.Features.Workspace.Queries.GetUserWorkspaces;

public class GetUserWorkspacesQuery : IRequest<IReadOnlyList<UserWorkspaceDto>>
{
}
