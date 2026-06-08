using MediatR;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Workspace.Queries.GetUserWorkspaces;

public class GetUserWorkspacesQueryHandler : 
    IRequestHandler<GetUserWorkspacesQuery, IReadOnlyList<UserWorkspaceDto>>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserContext _userContext;

    public GetUserWorkspacesQueryHandler(
        IWorkspaceRepository workspaceRepository,
        IUserContext userContext)
    {
        _workspaceRepository = workspaceRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<UserWorkspaceDto>> Handle(
        GetUserWorkspacesQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        return await _workspaceRepository.GetByUserIdAsync(
            currentUserId, 
            cancellationToken);
    }
}
