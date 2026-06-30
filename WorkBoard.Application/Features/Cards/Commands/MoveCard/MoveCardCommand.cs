using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.MoveCard;

public record MoveCardCommand(
    Guid BoardId,
    Guid CardId,
    Guid NewSectionId,
    double NewPosition) : IRequest<Unit>;
