using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Workspace.Queries.SearchAssignableUsers;

public class SearchWorkspaceAssignableUsersQueryHandler
    : IRequestHandler<SearchWorkspaceAssignableUsersQuery, IReadOnlyList<UserSearchDto>>
{
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly IUserContext _userContext;
    private readonly IBlobStorageService _blobStorageService;

    public SearchWorkspaceAssignableUsersQueryHandler(
        IWorkspaceMemberRepository workspaceMemberRepository,
        IUserContext userContext,
        IBlobStorageService blobStorageService)
    {
        _workspaceMemberRepository = workspaceMemberRepository;
        _userContext = userContext;
        _blobStorageService = blobStorageService;
    }

    public async Task<IReadOnlyList<UserSearchDto>> Handle(
        SearchWorkspaceAssignableUsersQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        if (string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            return new List<UserSearchDto>().AsReadOnly();
        }

        var usersFromDb = await _workspaceMemberRepository.SearchWorkspaceAssignableUsersAsync(
            request.WorkspaceId,
            request.SearchTerm,
            cancellationToken);

        foreach (var user in usersFromDb)
        {
            user.Initials = InitialGenerator.Generate(user.FullName);
            if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
            {
                user.AvatarUrl = _blobStorageService.GetReadSasUrl(
                    user.AvatarUrl,
                    BlobContainers.Avatars);
            }
        }

        return usersFromDb;
    }
}
