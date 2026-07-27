using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Features.Boards.Queries.SearchAssignableUsers;
using WorkBoard.Application.Features.User.Commands.RegisterUser;
using WorkBoard.Application.Features.User.Commands.UpdateAvatarColor;
using WorkBoard.Application.Features.User.Commands.UpdateAvatarImage;
using WorkBoard.Application.Features.User.Queries.GetCurrentUserProfile;

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
    [Authorize]
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

    /// <summary>
    /// Retrieves the profile of the currently authenticated user
    /// </summary>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// The profile data of the current user including avatar settings
    /// </returns>
    /// <response code="200">
    /// The profile was successfully retrieved
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpGet("current-user")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentUserProfile(
        CancellationToken cancellationToken)
    {
        var query = new GetCurrentUserProfileQuery();

        var result = await _mediator.Send(
            query, 
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates the avatar color of the currently authenticated user
    /// </summary>
    /// <param name="command">
    /// The command containing the new HEX color
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the request
    /// </param>
    /// <returns>
    /// No content upon successful update
    /// </returns>
    /// <response code="204">
    /// The avatar color was successfully updated
    /// </response>
    /// <response code="400">
    /// Bad request (invalid color format)
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="404">
    /// The user was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpPatch("avatar-color")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAvatarColor(
        [FromBody] UpdateUserAvatarColorCommand command,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Updates the avatar image of the currently authenticated user
    /// </summary>
    /// <param name="file">
    /// The image file to upload (max 5 MB)
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the request
    /// </param>
    /// <returns>
    /// The new avatar URL
    /// </returns>
    /// <response code="200">
    /// The avatar image was successfully updated
    /// </response>
    /// <response code="400">
    /// Bad request (e.g., file too large or not an image)
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpPatch("avatar-image")]
    [Authorize]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAvatarImage(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty or not provided.");
        }

        using var stream = file.OpenReadStream();

        var command = new UpdateUserAvatarImageCommand
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length
        };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}