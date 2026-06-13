using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Workspace.Queries.GetWorkspaceById;

public class GetWorkspaceByIdQueryHandler 
    : IRequestHandler<GetWorkspaceByIdQuery, UserWorkspaceDto>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetWorkspaceByIdQueryHandler(
        IWorkspaceRepository workspaceRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IUserContext userContext,
        IMapper mapper)
    {
        _workspaceRepository = workspaceRepository;
        _workspaceMemberRepository = workspaceMemberRepository;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<UserWorkspaceDto> Handle(
        GetWorkspaceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        var workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId, 
            cancellationToken);

        if (workspace == null)
        {
            throw new NotFoundException(
                $"Workspace with ID {request.WorkspaceId} was not found.");
        }

        var membership = await _workspaceMemberRepository.GetMembershipAsync(
            currentUserId,
            request.WorkspaceId,
            cancellationToken);

        if (membership == null)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to view this workspace.");
        }

        return _mapper.Map<UserWorkspaceDto>(workspace, opt =>
            opt.Items["UserRole"] = membership.UserRole);
    }
}
