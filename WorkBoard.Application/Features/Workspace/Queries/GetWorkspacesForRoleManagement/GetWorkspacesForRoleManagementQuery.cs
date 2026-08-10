using MediatR;
using WorkBoard.Application.Common.Dtos.Workspaces;

namespace WorkBoard.Application.Features.Workspace.Queries.GetWorkspacesForRoleManagement;

public record GetWorkspacesForRoleManagementQuery 
    : IRequest<IReadOnlyList<UserWorkspaceDto>>;
