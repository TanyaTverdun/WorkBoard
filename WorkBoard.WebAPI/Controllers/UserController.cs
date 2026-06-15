using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Features.User.Commands.RegisterUser;

namespace WorkBoard.WebAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly IUserContext _userContext;

    public UserController(
        ISender mediator,
        IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    /// <summary>
    /// Auth a user
    /// </summary>
    /// <param name="cancellationToken">
    /// A token to cancel the request
    /// </param>
    /// <returns>
    /// The unique identifier of the user
    /// </returns>
    [HttpPost("auth")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AuthenticateWithEntraId(
        CancellationToken cancellationToken)
    {
        if (_userContext.UserId == null)
        {
            return Unauthorized(
                "User context could not be resolved from the token");
        }

        var command = new AuthUserCommand
        {
            UserId = _userContext.UserId.Value,
            Email = _userContext.Email ?? string.Empty,
            FullName = _userContext.FullName
        };

        var localUserId = await _mediator.Send(
            command, 
            cancellationToken);

        return Ok(localUserId);
    }
}