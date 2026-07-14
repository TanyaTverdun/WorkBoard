namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistCreatedDto(
    Guid CardId, 
    ChecklistDto Checklist);
