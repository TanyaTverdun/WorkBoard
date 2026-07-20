using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Comments;
using WorkBoard.Application.Features.Comments.Commands.CreateComment;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/cards/{cardId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new comment on a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card to which the comment will be added
    /// </param>
    /// <param name="request">
    /// The comment details (Text)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The details of the newly created comment
    /// </returns>
    /// <response code="200">
    /// Returns the created comment successfully
    /// </response>
    /// <response code="400">
    /// If the input data is invalid (e.g., empty text)
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to comment on this card
    /// </response>
    /// <response code="404">
    /// If the card with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CommentDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateComment(
        [FromRoute] Guid cardId,
        [FromBody] CreateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCommentCommand
        {
            CardId = cardId,
            Text = request.Text
        };

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }
}
