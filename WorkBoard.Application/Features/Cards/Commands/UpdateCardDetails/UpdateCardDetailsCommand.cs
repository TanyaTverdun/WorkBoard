using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDetails;

public record UpdateCardDetailsCommand(
    Guid BoardId,
    Guid CardId,
    string? Title,
    string? Description,
    bool IsDescriptionUpdated) : IRequest<Unit>;
