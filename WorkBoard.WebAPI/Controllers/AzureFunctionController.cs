using Microsoft.AspNetCore.Mvc;
using WorkBoard.Application.Common.Dtos.Boards;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.WebAPI.Controllers;

[ApiController]
[Route("api/azureFunction")]
public class AzureFunctionController : ControllerBase
{
    private readonly IArchivationNotificationService _ArchivationNotificationService;
    private readonly IAppNotificationService _appNotificationService;

    public AzureFunctionController(
        IArchivationNotificationService notificationService, 
        IAppNotificationService appNotificationService)
    {
        _ArchivationNotificationService = notificationService;
        _appNotificationService = appNotificationService;
    }

    [HttpPost("{boardId:guid}/archivation-completed")]
    public async Task<IActionResult> ArchivationCompleted(Guid boardId)
    {
        await _ArchivationNotificationService.SendArchivationStatusChangedAsync(
            boardId,
            BoardArchiveStatus.Archived);

        return Ok();
    }

    [HttpPost("{boardId:guid}/restore-completed")]
    public async Task<IActionResult> RestoreCompleted(Guid boardId)
    {
        await _ArchivationNotificationService.SendArchivationStatusChangedAsync(
            boardId,
            BoardArchiveStatus.Active);

        var dto = new BoardArchiveStatusUpdatedDto
        {
            BoardId = boardId,
            IsArchived = false
        };

        await _appNotificationService.SendSidebarBoardStatusChangedAsync();

        return Ok();
    }
}
