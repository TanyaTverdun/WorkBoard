using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDescription;

public record UpdateCardDescriptionCommand(
    Guid BoardId,
    Guid CardId,
    string? Description) : IRequest<Unit>;

