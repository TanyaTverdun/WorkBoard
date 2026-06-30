using MediatR;
using WorkBoard.Application.Common.Dtos.Cards;

namespace WorkBoard.Application.Features.Cards.Commands.CreateCard;

public record CreateCardCommand : IRequest<CardDto>
{
    public required Guid SectionId { get; init; }
    public required string Title { get; init; }
    public required double Position { get; init; }
}
