using MediatR;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Application.Features.Boards.Commands.CreateBoard;
using WorkBoard.Application.Features.Boards.Queries.GetBoardsByWorkspace;

namespace WorkBoard.WebAPI.Controllers;

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
}
