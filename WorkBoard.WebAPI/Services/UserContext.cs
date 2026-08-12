using System.Security.Claims;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.WebAPI.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;

    private const string AzureOidClaim = 
        "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private const string AzurePreferredUsernameClaim = "preferred_username";
    private const string AzureNameClaim = "name";

    public Guid? CurrentWorkspaceId { get; private set; }
    public WorkspaceRole? CurrentWorkspaceRole { get; private set; }

    public UserContext(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IBlobStorageService blobStorageService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _workspaceMemberRepository = workspaceMemberRepository;
        _blobStorageService = blobStorageService;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var nameIdentifier = User?.FindFirst(AzureOidClaim)?.Value
                                 ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(nameIdentifier, out var parsedGuid) 
                ? parsedGuid : null;
        }
    }

    public string? Email => User?.FindFirst(AzurePreferredUsernameClaim)?.Value
                            ?? User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? FullName => User?.FindFirst(AzureNameClaim)?.Value
                               ?? User?.FindFirst(ClaimTypes.Name)?.Value;

    public async Task SetWorkspaceContextAsync(
        Guid workspaceId, 
        CancellationToken cancellationToken = default)
    {
        CurrentWorkspaceId = workspaceId;

        var userId = UserId;
        if (userId == null)
        {
            CurrentWorkspaceRole = null;
            return;
        }

        var membership = await _workspaceMemberRepository.GetMembershipAsync(
            userId.Value,
            workspaceId,
            cancellationToken);

        CurrentWorkspaceRole = membership?.UserRole;
    }

    public async Task<User?> GetCurrentUserFullProfileAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = UserId;
        if (userId == null)
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(
            userId.Value, 
            cancellationToken);

        if (!string.IsNullOrWhiteSpace(user.AvatarUrl))
        {
            user.AvatarUrl = _blobStorageService.GetReadSasUrl(
                user.AvatarUrl,
                BlobContainers.Avatars);
        }

        return user;
    }
}