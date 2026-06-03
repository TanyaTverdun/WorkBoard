using Microsoft.Identity.Web;
using System.Security.Claims;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.WebAPI.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string AzureOidClaim = 
        "http://schemas.microsoft.com/identity/claims/objectidentifier";
    private const string AzurePreferredUsernameClaim = "preferred_username";
    private const string AzureNameClaim = "name";

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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
}