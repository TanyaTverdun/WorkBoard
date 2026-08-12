using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Chat;
using WorkBoard.Application.Features.Chat.Queries.AskAi;

namespace WorkBoard.WebAPI.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly ISender _mediator;

    public ChatController(
        ISender mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sends a conversation history to the AI assistant and gets a response
    /// </summary>
    /// <param name="workspaceId">
    /// The unique identifier of the current workspace (passed via headers)
    /// </param>
    /// <param name="request">
    /// The chat history containing user and assistant messages
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the request
    /// </param>
    /// <returns>
    /// The generated text response from the AI
    /// </returns>
    /// <response code="200">
    /// The AI successfully generated a response
    /// </response>
    /// <response code="400">
    /// Bad request (e.g., missing Workspace ID)
    /// </response>
    /// <response code="401">
    /// The user is not authenticated
    /// </response>
    /// <response code="500">
    /// An internal server error occurred within the AI pipeline
    /// </response>
    [HttpPost("ask")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AskAi(
        [FromHeader(Name = "X-Workspace-Id")] Guid workspaceId,
        [FromBody] ChatRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = new AskAiQuery(
            workspaceId,
            request.Messages);

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(new ChatResponseDto 
        { 
            Answer = result 
        });
    }
}
