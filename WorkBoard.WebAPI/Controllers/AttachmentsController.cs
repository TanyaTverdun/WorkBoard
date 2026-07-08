using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Features.Attachments.Commands.AddAttachment;
using WorkBoard.Application.Features.Attachments.Queries.GetAttachmentsByCard;

namespace WorkBoard.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/cards/{cardId:guid}/attachments")]
public class AttachmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AttachmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all attachments attached to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// A list of attachments attached to the card
    /// </returns>
    /// <response code="200">
    /// Returns the list of attached attachments successfully
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAttachmentsByCard(
        [FromRoute] Guid cardId,
        CancellationToken cancellationToken)
    {
        var query = new GetAttachmentsByCardQuery
        {
            CardId = cardId
        };

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Uploads a new attachment to a specific card
    /// </summary>
    /// <param name="cardId">
    /// The unique identifier of the card
    /// </param>
    /// <param name="file">
    /// The file to be uploaded
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token provided by the runtime
    /// </param>
    /// <returns>
    /// The details of the newly uploaded attachment
    /// </returns>
    /// <response code="200">
    /// Returns the uploaded attachment successfully
    /// </response>
    /// <response code="400">
    /// If the file is missing, empty, or exceeds the size limit
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
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(100L * 1024 * 1024)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadAttachment(
        [FromRoute] Guid cardId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty or not provided.");
        }

        using var stream = file.OpenReadStream();

        var command = new AddAttachmentCommand
        {
            CardId = cardId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSizeBytes = file.Length,
            FileStream = stream
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}
