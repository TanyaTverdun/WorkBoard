using System.Security.Claims;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.WebAPI.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IBlobStorageService _blobStorageService;

    private const string ContainerName = "avatars";

    private const string AzureOidClaim = 
        "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private const string AzurePreferredUsernameClaim = "preferred_username";
    private const string AzureNameClaim = "name";

    public UserContext(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IBlobStorageService blobStorageService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
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
                ContainerName);
        }

        return user;
    }
}