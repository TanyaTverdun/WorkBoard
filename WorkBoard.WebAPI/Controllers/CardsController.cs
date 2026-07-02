using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Features.Cards.Commands.AddCardAssignee;
using WorkBoard.Application.Features.Cards.Commands.CreateCard;
using WorkBoard.Application.Features.Cards.Commands.DeleteCard;
using WorkBoard.Application.Features.Cards.Commands.DeleteCardAssignee;
using WorkBoard.Application.Features.Cards.Commands.MoveCard;
using WorkBoard.Application.Features.Cards.Commands.UpdateCardDescription;
using WorkBoard.Application.Features.Cards.Commands.UpdateCardTitle;
using WorkBoard.Application.Features.Cards.Queries.GetAssignableUsers;
using WorkBoard.Application.Features.Cards.Queries.GetCardAssignees;
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

    /// <summary>
    /// Moves a card to a new section or changes its position
    /// </summary>
    /// <param name="boardId">
    /// The ID of the board
    /// </param>
    /// <param name="cardId">
    /// The ID of the card to move
    /// </param>
    /// <param name="request">
    /// The new section ID and position
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <response code="200">
    /// Card moved successfully
    /// </response>
    /// <response code="400">
    /// Invalid data
    /// </response>
    /// <response code="403">
    /// Forbidden
    /// </response>
    /// <response code="404">
    /// Card or Section not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPatch("/api/boards/{boardId:guid}/cards/{cardId:guid}/move")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveCard(
        [FromRoute] Guid boardId,
        [FromRoute] Guid cardId,
        [FromBody] MoveCardRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MoveCardCommand(
            boardId,
            cardId,
            request.NewSectionId,
            request.NewPosition
        );

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Updates the title of a card
    /// </summary>
    /// <param name="boardId">
    /// The ID of the board
    /// </param>
    /// <param name="cardId">
    /// The ID of the card to update
    /// </param>
    /// <param name="request">
    /// The new title
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <response code="204">
    /// Title updated successfully
    /// </response>
    /// <response code="400">
    /// Invalid data
    /// </response>
    /// <response code="403">
    /// Forbidden
    /// </response>
    /// <response code="404">
    /// Card or Board not found
    /// </response>
    /// <response code="500">
    /// Internal server error
    /// </response>
    [HttpPut("/api/boards/{boardId:guid}/cards/{cardId:guid}/title")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCardTitle(
        [FromRoute] Guid boardId,
        [FromRoute] Guid cardId,
        [FromBody] UpdateCardTitleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCardTitleCommand(boardId, cardId, request.Title);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Updates the description of a card
    /// </summary>
    /// <param name="boardId">
    /// The ID of the board
    /// </param>
    /// <param name="cardId">
    /// The ID of the card to update
    /// </param>
    /// <param name="request">
    /// The new description
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <response code="204">
    /// Description updated successfully
    /// </response>
    /// <response code="400">
    /// Invalid data
    /// </response>
    /// <response code="403">
    /// Forbidden
    /// </response>
    /// <response code="404">
    /// Card or Board not found
    /// </response>
    /// <response code="500">
    /// Internal server error
    /// </response>
    [HttpPut("/api/boards/{boardId:guid}/cards/{cardId:guid}/description")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateCardDescription(
        [FromRoute] Guid boardId,
        [FromRoute] Guid cardId,
        [FromBody] UpdateCardDescriptionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCardDescriptionCommand(boardId, cardId, request.Description);

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific card from the board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="cardId">
    /// The unique identifier of the card to delete
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <response code="204">
    /// Indicates that the card was deleted successfully
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to delete cards on this board
    /// </response>
    /// <response code="404">
    /// If the card with the specified ID was not found
    /// </response>
    [HttpDelete("/api/boards/{boardId:guid}/cards/{cardId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCard(
        [FromRoute] Guid boardId,
        [FromRoute] Guid cardId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCardCommand
        {
            BoardId = boardId,
            CardId = cardId
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Retrieves a list of all users assigned to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of assigned users with their basic information and initials
    /// </returns>
    /// <response code="200">
    /// Returns the list of card assignees successfully
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have permission to access this board
    /// </response>
    /// <response code="404">
    /// If the card with the specified ID or its section was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpGet("/api/cards/{cardId:guid}/assignees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCardAssignees(
        [FromRoute] Guid cardId,
        CancellationToken cancellationToken)
    {
        var query = new GetCardAssigneesQuery 
        { 
            CardId = cardId 
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Assigns a user to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="request">
    /// The object containing the user ID to assign
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <response code="200">
    /// The user was successfully assigned to the card
    /// </response>
    /// <response code="400">
    /// If the user is already assigned or is not a member of the board
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the current user does not have permission to modify this card
    /// </response>
    /// <response code="404">
    /// If the card or its section was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpPost("/api/cards/{cardId:guid}/assignees")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAssignee(
        [FromRoute] Guid cardId,
        [FromBody] AddCardAssigneeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCardAssigneeCommand
        {
            CardId = cardId,
            TargetUserId = request.UserId
        };

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Gets a list of board members who are not yet assigned to this card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of users available for assignment
    /// </returns>
    /// <response code="200">
    /// Returns the list successfully
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have access to this board
    /// </response>
    /// <response code="404">
    /// If the card or its section was not found
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpGet("/api/cards/{cardId:guid}/assignable-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAssignableUsers(
        [FromRoute] Guid cardId,
        CancellationToken cancellationToken)
    {
        var query = new GetAssignableUsersQuery(cardId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Removes a specific assigned user from a card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card from which the user will be removed
    /// </param>
    /// <param name="userId">
    /// The unique identifier of the user to be removed from the card
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// No content upon successful removal
    /// </returns>
    /// <response code="204">
    /// Returns successfully with no content when the user is unassigned
    /// </response>
    /// <response code="401">
    /// If the user is not authenticated
    /// </response>
    /// <response code="403">
    /// If the user does not have access to modify this card
    /// </response>
    /// <response code="404">
    /// If the card was not found, or the target user is not assigned to this card
    /// </response>
    /// <response code="500">
    /// If an internal server error occurs while processing the request
    /// </response>
    [HttpDelete("/api/cards/{cardId:guid}/assignees/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveAssignee(
        [FromRoute] Guid cardId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCardAssigneeCommand
        {
            CardId = cardId,
            TargetUserId = userId
        };

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
