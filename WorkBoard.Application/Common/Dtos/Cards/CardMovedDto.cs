namespace WorkBoard.Application.Common.Dtos.Cards;

public record CardMovedDto(
    Guid CardId,
    Guid NewSectionId,
    string NewSectionName,
    double NewPosition);
