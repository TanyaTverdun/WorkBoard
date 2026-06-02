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

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class
    /// </summary>
    /// <param name="mediator">
    /// The MediatR sender to dispatch commands and queries
    /// </param>
    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="command">
    /// The registration data
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the request
    /// </param>
    /// <returns>
    /// The unique identifier of the user
    /// </returns>
    [HttpPost("register")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterUser(
        [FromBody] RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        var userId = await _mediator
            .Send(
                command, 
                cancellationToken);

        return Ok(userId);
    }
}