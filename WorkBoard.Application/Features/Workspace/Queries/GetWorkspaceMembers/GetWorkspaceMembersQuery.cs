using MediatR;
using WorkBoard.Application.Common.Dtos.Workspaces;

namespace WorkBoard.Application.Features.Workspace.Queries.GetWorkspaceMembers;

public record GetWorkspaceMembersQuery(Guid WorkspaceId) 
    : IRequest<IReadOnlyList<WorkspaceMemberDto>>;
