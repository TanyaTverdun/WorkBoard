namespace WorkBoard.Application.Common.Dtos.Cards;

public record CardRenameDto(
    Guid CardId, 
    string NewTitle);
