using MediatR;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Workspace.Queries.GetWorkspacesForRoleManagement;

public class GetWorkspacesForRoleManagementQueryHandler
    : IRequestHandler<GetWorkspacesForRoleManagementQuery, IReadOnlyList<UserWorkspaceDto>>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserContext _userContext;

    public GetWorkspacesForRoleManagementQueryHandler(
        IWorkspaceRepository workspaceRepository,
        IUserContext userContext)
    {
        _workspaceRepository = workspaceRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<UserWorkspaceDto>> Handle(
        GetWorkspacesForRoleManagementQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        return await _workspaceRepository.GetWorkspacesForRoleManagementAsync(
            currentUserId,
            cancellationToken);
    }
}
