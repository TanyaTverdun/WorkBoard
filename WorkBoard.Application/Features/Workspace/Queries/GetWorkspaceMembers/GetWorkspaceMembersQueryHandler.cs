using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Workspace.Queries.GetWorkspaceMembers;

public class GetWorkspaceMembersQueryHandler
    : IRequestHandler<GetWorkspaceMembersQuery, IReadOnlyList<WorkspaceMemberDto>>
{
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly IUserContext _userContext;
    private readonly IBlobStorageService _blobStorageService;

    public GetWorkspaceMembersQueryHandler(
        IWorkspaceMemberRepository workspaceMemberRepository,
        IUserContext userContext,
        IBlobStorageService blobStorageService)
    {
        _workspaceMemberRepository = workspaceMemberRepository;
        _userContext = userContext;
        _blobStorageService = blobStorageService;
    }

    public async Task<IReadOnlyList<WorkspaceMemberDto>> Handle(
        GetWorkspaceMembersQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        var isMember = await _workspaceMemberRepository.IsMemberAsync(
            request.WorkspaceId,
            currentUserId,
            cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this workspace.");
        }

        var members = await _workspaceMemberRepository.GetMembersByWorkspaceIdAsync(
            request.WorkspaceId,
            cancellationToken);

        foreach (var member in members)
        {
            member.IsCurrentUser = member.Id == currentUserId;
            member.Initials = InitialGenerator.Generate(member.Name);

            if (!string.IsNullOrWhiteSpace(member.AvatarUrl))
            {
                member.AvatarUrl = _blobStorageService.GetReadSasUrl(
                    member.AvatarUrl,
                    BlobContainers.Avatars);
            }
        }

        return members;
    }
}
