namespace WorkBoard.Application.Common.Dtos.Cards;

public record CardDescriptionUpdateDto(
    Guid CardId, 
    string? Description);
