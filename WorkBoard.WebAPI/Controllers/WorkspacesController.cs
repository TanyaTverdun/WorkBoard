using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Features.Workspace.Commands.CreateWorkspace;
using WorkBoard.Application.Features.Workspace.Queries.GetUserWorkspaces;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkspacesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspacesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new workspace
    /// </summary>
    /// <param name="command">
    /// The workspace creation details (Name)
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// The unique identifier of the newly created workspace
    /// </returns>
    /// <response code="200">
    /// Success. Returns the Guid of the created workspace
    /// </response>
    /// <response code="400">
    /// Bad Request. If the workspace name is invalid or exceeds 50 characters
    /// </response>
    /// <response code="401">
    /// Unauthorized. If the user is not authenticated via Azure Entra ID
    /// </response>
    /// <response code="500">
    /// Internal Server Error. If a database or connection error occurs
    /// </response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreateWorkspaceCommand command, 
        CancellationToken cancellationToken)
    {
        var workspaceId = await _mediator.Send(command, cancellationToken);

        return Ok(workspaceId);
    }

    /// <summary>
    /// Retrieves all workspaces where the current user is a member
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// A list of workspaces with the user's role and subscription tier
    /// </returns>
    /// <response code="200">
    /// Success. Returns the list of workspaces
    /// </response>
    /// <response code="401">
    /// Unauthorized. If the user is not authenticated via Azure Entra ID
    /// </response>
    /// <response code="500">
    /// Internal Server Error. If a database error occurs
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserWorkspaceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<UserWorkspaceDto>>> GetMyWorkspaces(
        CancellationToken cancellationToken)
    {
        var query = new GetUserWorkspacesQuery();
        var workspaces = await _mediator.Send(query, cancellationToken);

        return Ok(workspaces);
    }
}
