namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistItemRenamedDto(
    Guid ChecklistId, 
    Guid ItemId, 
    string NewTitle);
