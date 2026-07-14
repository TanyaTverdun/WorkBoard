namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistItemStatusUpdatedDto(
    Guid ChecklistId, 
    Guid ItemId, 
    bool IsDone);
