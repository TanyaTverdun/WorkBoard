using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Features.Checklists.Commands.AddChecklistItem;
using WorkBoard.Application.Features.Checklists.Commands.CreateChecklist;
using WorkBoard.Application.Features.Checklists.Commands.DeleteChecklist;
using WorkBoard.Application.Features.Checklists.Commands.UpdateChecklist;
using WorkBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;
using WorkBoard.Application.Features.Checklists.Commands.UpdateChecklistItemStatus;
using WorkBoard.Application.Features.Checklists.Queries.GetChecklistsByCard;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/cards/{cardId:guid}/checklists")]
public class ChecklistsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChecklistsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all checklists attached to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of checklists belonging to the card
    /// </returns>
    /// <response code="200">
    /// Returns the list of checklists successfully
    /// </response>
    /// </response code="204">
    /// Return no content if the card has no checklists
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
    [ProducesResponseType(typeof(ChecklistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChecklistByCard(
    [FromRoute] Guid cardId,
    CancellationToken cancellationToken)
    {
        var query = new GetChecklistByCardQuery(cardId);

        var result = await _mediator.Send(query, cancellationToken);

        if (result == null)
        {
            return NoContent();
        }

        return Ok(result);
    }

    /// <summary>
    /// Creates a new checklist for a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card to which the checklist will belong
    /// </param>
    /// <param name="request">
    /// The checklist details (Name)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The details of the newly created checklist
    /// </returns>
    /// <response code="200">
    /// Returns the created checklist successfully
    /// </response>
    /// <response code="400">
    /// If the input data is invalid
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this card
    /// </response>
    /// <response code="404">
    /// If the card with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChecklistDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateChecklist(
        [FromRoute] Guid cardId,
        [FromBody] CreateChecklistRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateChecklistCommand
        {
            CardId = cardId,
            Name = request.Name
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates the name of an existing checklist
    /// </summary>
    /// <param name="checklistId">
    /// The unique identifier of the checklist to update
    /// </param>
    /// <param name="request">
    /// The updated details for the checklist (Name)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The updated checklist details
    /// </returns>
    /// <response code="200">
    /// Returns the updated checklist successfully
    /// </response>
    /// <response code="400">
    /// If the provided data is invalid
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this checklist
    /// </response>
    /// <response code="404">
    /// If the checklist with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPut("/api/checklists/{checklistId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChecklistDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateChecklist(
        [FromRoute] Guid checklistId,
        [FromBody] UpdateChecklistRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateChecklistCommand
        {
            ChecklistId = checklistId,
            Name = request.Name
        };

        var result = await _mediator.Send(
            command, 
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Deletes a checklist permanently
    /// </summary>
    /// <param name="checklistId">
    /// The unique identifier of the checklist to delete
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <response code="204">
    /// Indicates that the checklist was deleted successfully
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this checklist
    /// </response>
    /// <response code="404">
    /// If the checklist with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpDelete("/api/checklists/{checklistId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteChecklist(
        [FromRoute] Guid checklistId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteChecklistCommand
        {
            ChecklistId = checklistId
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Adds a new item to an existing checklist
    /// </summary>
    /// <param name="checklistId">
    /// The unique identifier of the checklist
    /// </param>
    /// <param name="request">
    /// The details of the new checklist item (Title)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The details of the newly created checklist item
    /// </returns>
    /// <response code="200">
    /// Returns the created checklist item successfully
    /// </response>
    /// <response code="400">
    /// If the input data is invalid (Title is empty or duplicate)
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this checklist
    /// </response>
    /// <response code="404">
    /// If the checklist with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPost("/api/checklists/{checklistId:guid}/items")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChecklistItemDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddChecklistItem(
        [FromRoute] Guid checklistId,
        [FromBody] AddChecklistItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddChecklistItemCommand
        {
            ChecklistId = checklistId,
            Title = request.Title
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates the execution status (IsDone) of a checklist item
    /// </summary>
    /// <param name="itemId">
    /// The unique identifier of the checklist item
    /// </param>
    /// <param name="request">
    /// The new status (IsDone)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The updated checklist item details
    /// </returns>
    /// <response code="200">
    /// Returns the updated checklist item successfully
    /// </response>
    /// <response code="400">
    /// If the input data is invalid
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this checklist
    /// </response>
    /// <response code="404">
    /// If the checklist item with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPut("/api/checklists/items/{itemId:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChecklistItemDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateChecklistItemStatus(
        [FromRoute] Guid itemId,
        [FromBody] UpdateChecklistItemStatusRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateChecklistItemStatusCommand
        {
            ItemId = itemId,
            IsDone = request.IsDone
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates the title of an existing checklist item
    /// </summary>
    /// <param name="itemId">
    /// The unique identifier of the checklist item to update
    /// </param>
    /// <param name="request">
    /// The updated details for the checklist item (Title)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The updated checklist item details
    /// </returns>
    /// <response code="200">
    /// Returns the updated checklist item successfully
    /// </response>
    /// <response code="400">
    /// If the provided data is invalid or duplicate title exists
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to modify this checklist item
    /// </response>
    /// <response code="404">
    /// If the checklist item with the specified ID was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPut("/api/checklists/items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChecklistItemDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateChecklistItem(
        [FromRoute] Guid itemId,
        [FromBody] UpdateChecklistItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateChecklistItemCommand
        {
            ItemId = itemId,
            Title = request.Title
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
