using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Features.Labels.Commands.AddLabelToCard;
using WorkBoard.Application.Features.Labels.Commands.CreateLabel;
using WorkBoard.Application.Features.Labels.Commands.DeleteLabel;
using WorkBoard.Application.Features.Labels.Commands.RemoveLabelFromCard;
using WorkBoard.Application.Features.Labels.Commands.UpdateLabel;
using WorkBoard.Application.Features.Labels.Queries.GetLabelsByBoard;
using WorkBoard.Application.Features.Labels.Queries.GetLabelsByCard;

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

    /// <summary>
    /// Gets all labels available on a specific board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of all labels belonging to the specified board
    /// </returns>
    /// <response code="200">
    /// Returns the list of labels successfully
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
    [HttpGet("/api/boards/{boardId:guid}/labels")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<LabelDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLabelsByBoard(
        [FromRoute] Guid boardId,
        CancellationToken cancellationToken)
    {
        var query = new GetLabelsByBoardQuery
        {
            BoardId = boardId
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets all labels attached to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of labels attached to the card
    /// </returns>
    /// <response code="200">
    /// Returns the list of attached labels successfully
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<LabelDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLabelsByCard(
        [FromRoute] Guid cardId,
        CancellationToken cancellationToken)
    {
        var query = new GetLabelsByCardQuery
        {
            CardId = cardId
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }


    /// <summary>
    /// Attaches an existing label to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="labelId">
    /// The unique identifier of the label to attach
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <response code="204">
    /// Indicates that the label was attached successfully
    /// </response>
    /// <response code="400">
    /// If the provided data is invalid
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this card
    /// </response>
    /// <response code="404">
    /// If the card or label with the specified ID was not found
    /// </response>
    [HttpPost("{labelId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddLabelToCard(
        [FromRoute] Guid cardId,
        [FromRoute] Guid labelId,
        CancellationToken cancellationToken)
    {
        var command = new AddLabelToCardCommand
        {
            CardId = cardId,
            LabelId = labelId
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Removes an attached label from a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="labelId">
    /// The unique identifier of the label to remove
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <response code="204">
    /// Indicates that the label was removed from the card successfully
    /// </response>
    /// <response code="400">
    /// If the provided data is invalid
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this card
    /// </response>
    /// <response code="404">
    /// If the card or label with the specified ID was not found
    /// </response>
    [HttpDelete("{labelId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveLabelFromCard(
        [FromRoute] Guid cardId,
        [FromRoute] Guid labelId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveLabelFromCardCommand
        {
            CardId = cardId,
            LabelId = labelId
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Updates the details of an existing label
    /// </summary>
    /// <param name="labelId">
    /// The unique identifier of the label to update
    /// </param>
    /// <param name="request">
    /// The updated details for the label (Name and Color)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The updated label details
    /// </returns>
    /// <response code="200">
    /// Returns the updated label successfully
    /// </response>
    /// <response code="400">
    /// If the provided data is invalid (name already exists on the board)
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify labels on this board
    /// </response>
    /// <response code="404">
    /// If the label with the specified ID was not found
    /// </response>
    [HttpPut("/api/labels/{labelId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LabelDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLabel(
        [FromRoute] Guid labelId,
        [FromBody] UpdateLabelRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLabelCommand
        {
            LabelId = labelId,
            Name = request.Name,
            Color = request.Color
        };

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return Ok(result);
    }

    
}
