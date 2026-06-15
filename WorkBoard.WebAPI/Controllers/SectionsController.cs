using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Application.Features.Sections.Queries.GetSectionsByBoard;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/boards/{boardId:guid}/sections")]
public class SectionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SectionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all active sections for a specific board ordered by their position
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of sections belonging to the specified board
    /// </returns>
    /// <response code="200">
    /// Returns the list of board sections successfully
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user is authenticated but does not have access to this board
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, 
        Type = typeof(IReadOnlyList<SectionDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<SectionDto>>> GetByBoardId(
        [FromRoute] Guid boardId,
        CancellationToken cancellationToken)
    {
        var query = new GetSectionsByBoardQuery(boardId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}
