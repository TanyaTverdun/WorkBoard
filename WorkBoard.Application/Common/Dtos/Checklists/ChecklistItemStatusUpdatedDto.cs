namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistItemStatusUpdatedDto(
    Guid CardId,
    Guid ChecklistId,
    Guid ItemId, 
    bool IsDone);
