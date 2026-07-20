namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistItemAddedDto(
    Guid ChecklistId, 
    ChecklistItemDto Item);
