namespace WorkBoard.Application.Common.Dtos.Cards;

public record AssigneeAddDto(
    Guid CardId, 
    CardAssigneeDto Assignee);
