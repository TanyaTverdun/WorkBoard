using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Application.Common.Dtos.BoardMembers;
using WorkBoard.Application.Features.Boards.Commands.AddBoardMember;
using WorkBoard.Application.Features.Boards.Commands.CreateBoard;
using WorkBoard.Application.Features.Boards.Commands.DeleteBoard;
using WorkBoard.Application.Features.Boards.Commands.RemoveBoardMember;
using WorkBoard.Application.Features.Boards.Commands.UpdateBoard;
using WorkBoard.Application.Features.Boards.Commands.UpdateMemberRole;
using WorkBoard.Application.Features.Boards.Queries.GetBoardById;
using WorkBoard.Application.Features.Boards.Queries.GetBoardMembers;
using WorkBoard.Application.Features.Boards.Queries.GetBoardsByWorkspace;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/workspaces/{workspaceId:guid}/boards")]
public class BoardsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BoardsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a list of all active boards within a workspace
    /// </summary>
    /// <param name="workspaceId">
    /// The unique identifier of the workspace
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// A list of boards with basic information and the user's role
    /// </returns>
    /// <response code="200">
    /// The list of boards was successfully retrieved
    /// </response>
    /// <response code="401">
    /// The user is not authenticated within the system
    /// </response>
    /// <response code="403">
    /// The user is not a member of this workspace
    /// </response>
    /// <response code="500">
    /// An internal server error occurred while processing
    /// the request or a database failure happened
    /// </response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IReadOnlyList<BoardDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<BoardDto>>> GetBoards(
        Guid workspaceId,
        CancellationToken cancellationToken)
    {
        var query = new GetBoardsByWorkspaceQuery(workspaceId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new board within the specified workspace.
    /// </summary>
    /// <param name="workspaceId">
    /// The unique identifier of the workspace where the board will be created
    /// </param>
    /// <param name="request">
    /// The object containing the name of the new board
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// The unique identifier of the newly created board
    /// </returns>
    /// <response code="201">
    /// The board was successfully created
    /// </response>
    /// <response code="400">
    /// The request data is invalid
    /// </response>
    /// <response code="401">
    /// The user is not authenticated within the system
    /// </response>
    /// <response code="403">
    /// The user is not a member of this workspace and cannot create boards here
    /// </response>
    /// <response code="500">
    /// An internal server error occurred while processing the request
    /// </response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateBoard(
        Guid workspaceId,
        [FromBody] CreateBoardRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBoardCommand(
            workspaceId, 
            request.Name);

        var boardId = await _mediator.Send(
            command, 
            cancellationToken);

        return Ok(boardId);
    }
    
    /// <summary>
    /// Deletes an existing board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board to delete
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// An empty OK response if the deletion was successful
    /// </returns>
    /// <response code="200">
    /// The board was successfully deleted
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="403">
    /// The user is  an observer in the workspace
    /// </response>
    /// <response code="404">
    /// The specified board was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpDelete("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteBoard(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteBoardCommand(boardId);
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
    
    /// <summary>
    /// Updates an existing board's details
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board to update
    /// </param>
    /// <param name="request">
    /// The object containing the updated board data
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// An empty OK response if the update was successful
    /// </returns>
    /// <response code="200">
    /// The board was successfully updated
    /// </response>
    /// <response code="400">
    /// The request data is invalid
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="403">
    /// The user is an observer or does not belong to the workspace
    /// </response>
    /// <response code="404">
    /// The specified board was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpPut("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateBoard(
        Guid boardId,
        [FromBody] UpdateBoardRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBoardCommand(boardId, request.Name);
        
        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Retrieves a specific board by its unique identifier
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board to retrieve
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// The details of the requested board
    /// </returns>
    /// <response code="200">
    /// The board details were successfully retrieved
    /// </response>
    /// <response code="401">
    /// The user is not authenticated within the system
    /// </response>
    /// <response code="403">
    /// The user does not have access to this board
    /// </response>
    /// <response code="404">
    /// The specified board was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred while processing the request
    /// </response>
    [HttpGet("{boardId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BoardDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BoardDto>> GetBoard(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        var query = new GetBoardByIdQuery(boardId);

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets all members of a specific board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board to get members for
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// A list of board members with their details and roles
    /// </returns>
    /// <response code="200">
    /// The list of board members was successfully retrieved
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="403">
    /// The user does not belong to this board
    /// </response>
    /// <response code="404">
    /// The specified board was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpGet("{boardId:guid}/members")]
    [ProducesResponseType(typeof(IReadOnlyList<BoardMemberDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBoardMembers(
        Guid boardId,
        CancellationToken cancellationToken)
    {
        var query = new GetBoardMembersQuery(boardId);

        var result = await _mediator.Send(
            query, 
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Updates the role of a specific member within a board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="userId">
    /// The unique identifier of the target user whose role is being updated
    /// </param>
    /// <param name="request">
    /// The object containing the new role for the member
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// An empty OK response if the update was successful
    /// </returns>
    /// <response code="200">
    /// The member's role was successfully updated
    /// </response>
    /// <response code="400">
    /// The request data is invalid
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="403">
    /// The user does not have permission to change roles
    /// </response>
    /// <response code="404">
    /// The specified board or member was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpPatch("{boardId:guid}/members/{userId:guid}/role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateMemberRole(
        Guid boardId,
        Guid userId,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateMemberRoleCommand(
            boardId, 
            userId, 
            request.NewRole);

        await _mediator.Send(
            command, 
            cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Adds a new member to the specified board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="request">
    /// The object containing the user ID and the role to be assigned
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel the operation
    /// </param>
    /// <returns>
    /// An empty OK response if the member was successfully added
    /// </returns>
    /// <response code="200">
    /// The member was successfully added to the board
    /// </response>
    /// <response code="400">
    /// The request data is invalid, or the user is already a member of the board
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="403">
    /// The user does not have permission to add members to this board
    /// </response>
    /// <response code="404">
    /// The specified board was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpPost("{boardId:guid}/members")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddBoardMember(
        Guid boardId,
        [FromBody] AddMemberRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddBoardMemberCommand(
            boardId,
            request.UserId,
            request.Role);

        await _mediator.Send(
            command, 
            cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Removes a member from the specified board
    /// </summary>
    /// <param name="boardId">
    /// The unique identifier of the board
    /// </param>
    /// <param name="userId">
    /// The unique identifier of the user to remove
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token
    /// </param>
    /// <returns>
    /// An empty OK response if the member was successfully removed
    /// </returns>
    /// <response code="200">
    /// The member was successfully removed from the board
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="403">
    /// The user does not have permission to remove members
    /// </response>
    /// <response code="404">
    /// The board or the member was not found
    /// </response>
    /// <response code="500">
    /// An internal server error occurred
    /// </response>
    [HttpDelete("{boardId:guid}/members/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveBoardMember(
        Guid boardId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveBoardMemberCommand(
            boardId, 
            userId);

        await _mediator.Send(
            command, 
            cancellationToken);

        return Ok();
    }
}      
