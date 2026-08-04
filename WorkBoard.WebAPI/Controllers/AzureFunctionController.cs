using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.WebAPI.Controllers;

[ApiController]
[Route("api/azureFunction")]
public class AzureFunctionController : ControllerBase
{
    private readonly IArchivationNotificationService _notificationService;

    public AzureFunctionController(IArchivationNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("{boardId:guid}/archivation-completed")]
    public async Task<IActionResult> ArchivationCompleted(Guid boardId)
    {
        await _notificationService.SendArchivationStatusChangedAsync(
            boardId,
            BoardArchiveStatus.Archived);

        return Ok();
    }

    [HttpPost("{boardId:guid}/restore-completed")]
    public async Task<IActionResult> RestoreCompleted(Guid boardId)
    {
        await _notificationService.SendArchivationStatusChangedAsync(
            boardId,
            BoardArchiveStatus.Active);

        return Ok();
    }
}
