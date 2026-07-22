using System.Security.Claims;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.WebAPI.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private User? _cachedUser;

    private const string AzureOidClaim = 
        "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private const string AzurePreferredUsernameClaim = "preferred_username";
    private const string AzureNameClaim = "name";

    public UserContext(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
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
        if (_cachedUser != null)
        {
            return _cachedUser;
        }

        var userId = UserId;
        if (userId == null)
        {
            return null;
        }

        _cachedUser = await _userRepository.GetByIdAsync(
            userId.Value, 
            cancellationToken);

        return _cachedUser;
    }
}