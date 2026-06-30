using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardTitle;

public record UpdateCardTitleCommand(
    Guid BoardId,
    Guid CardId,
    string Title) : IRequest<Unit>;
