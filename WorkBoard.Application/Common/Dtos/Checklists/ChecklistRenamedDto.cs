namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistRenamedDto(
    Guid ChecklistId, 
    string NewName);
