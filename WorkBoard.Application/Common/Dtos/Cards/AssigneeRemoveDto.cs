namespace WorkBoard.Application.Common.Dtos.Cards;

public record AssigneeRemoveDto(
    Guid CardId, 
    Guid UserId);
