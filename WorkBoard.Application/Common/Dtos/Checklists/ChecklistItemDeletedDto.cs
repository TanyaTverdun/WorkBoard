namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistItemDeletedDto(
    Guid ChecklistId, 
    Guid ItemId);
