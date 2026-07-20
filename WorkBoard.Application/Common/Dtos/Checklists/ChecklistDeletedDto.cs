namespace WorkBoard.Application.Common.Dtos.Checklists;

public record ChecklistDeletedDto(
    Guid CardId, 
    Guid ChecklistId);
