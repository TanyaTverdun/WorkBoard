using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Features.Boards.Queries.SearchAssignableUsers;
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

    /// <summary>
    /// Searches for users who can be added to the specified board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="searchTerm">
    /// The email prefix to search for among available users
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// A list of users matching the search criteria 
    /// who are not yet members of the board
    /// </returns>
    /// <response code="200">
    /// The list of assignable users was successfully retrieved
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="404">
    /// The specified board was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpGet("{boardId:guid}/assignable-users")]
    [ProducesResponseType(typeof(IReadOnlyList<UserSearchDto>), 
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchAssignableUsers(
        Guid boardId,
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var query = new SearchAssignableUsersQuery(boardId, searchTerm);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}