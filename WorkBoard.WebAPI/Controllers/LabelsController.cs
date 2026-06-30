using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Features.Labels.Commands.CreateLabel;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/cards/{cardId:guid}/labels")]
public class LabelsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LabelsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new label on the board and attaches it to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card to which the label will be attached
    /// </param>
    /// <param name="request">
    /// The label details (Name and optional Color)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The details of the newly created label
    /// </returns>
    /// <response code="200">
    /// Returns the created label successfully
    /// </response>
    /// <response code="400">
    /// If the input data is invalid  name is too long, invalid color format, 
    /// or name already exists)
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this board
    /// </response>
    /// <response code="404">
    /// If the card with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LabelDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateLabel(
        [FromRoute] Guid cardId,
        [FromBody] CreateLabelRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLabelCommand
        {
            CardId = cardId,
            Name = request.Name,
            Color = request.Color
        };

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }
}
