using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Features.Comments.GetCommentsByCard;

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
    /// Gets all comments attached to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of comments attached to the card
    /// </returns>
    /// <response code="200">
    /// Returns the list of comments successfully
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to access this card
    /// </response>
    /// <response code="404">
    /// If the card with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCommentsByCard(
        [FromRoute] Guid cardId,
        CancellationToken cancellationToken)
    {
        var query = new GetCommentsByCardQuery(cardId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}
