using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Features.Cards.Commands.CreateCard;
using WorkBoard.Application.Features.Cards.Queries.GetCardsByBoard;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/sections/{sectionId:guid}/cards")]
public class CardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new card within a specific section
    /// </summary>
    /// <param name="sectionId">
    /// The unique identifier of the section where the card will be created
    /// </param>
    /// <param name="request">
    /// The card details (Title and Position)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The details of the newly created card
    /// </returns>
    /// <response code="200">
    /// Returns the created card successfully
    /// </response>
    /// <response code="400">
    /// If the input data is invalid or title length exceeds limits
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this board
    /// </response>
    /// <response code="404">
    /// If the section with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(CardDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromRoute] Guid sectionId,
        [FromBody] CreateCardRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCardCommand
        {
            SectionId = sectionId,
            Title = request.Title,
            Position = request.Position
        };

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets all cards for a specific board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A flat list of all cards belonging to the specified board
    /// </returns>
    /// <response code="200">
    /// Returns the list of cards successfully
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to access this board
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpGet("/api/boards/{boardId:guid}/cards")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(IReadOnlyList<CardDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCardsByBoard(
        [FromRoute] Guid boardId,
        CancellationToken cancellationToken)
    {
        var query = new GetCardsByBoardQuery(boardId);

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }
}
