namespace WorkBoard.Application.Common.Dtos.Cards;

public record MoveCardRequest(
    Guid NewSectionId,
    double NewPosition);