namespace WorkBoard.Application.Common.Dtos.Cards;

public record UpdateCardDetailsRequest(
    string? Title,
    string? Description,
    bool IsDescriptionUpdated);
