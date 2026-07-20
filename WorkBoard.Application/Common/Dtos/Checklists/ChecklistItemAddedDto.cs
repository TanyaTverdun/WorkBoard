namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistItemAddedDto(
    Guid CardId,
    Guid ChecklistId, 
    ChecklistItemDto Item);
