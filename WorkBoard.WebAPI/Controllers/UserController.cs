using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Features.User.Commands.RegisterUser;

namespace WorkBoard.WebAPI.Controllers;

/// <summary>
/// Provides endpoints for managing user profiles
/// and authentication state
/// </summary>
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Auth a new user
    /// </summary>
    /// <param name="command">
    /// The auth data
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the request
    /// </param>
    /// <returns>
    /// The unique identifier of the user
    /// </returns>
    [HttpPost("outh")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AuthUser(
        [FromBody] AuthUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = await _mediator
            .Send(
                command, 
                cancellationToken);

        return Ok(userId);
    }
}