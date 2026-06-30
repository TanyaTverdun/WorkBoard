using MediatR;
using WorkBoard.Application.Common.Dtos.Cards;

namespace WorkBoard.Application.Features.Cards.Queries.GetCardsByBoard;

public record GetCardsByBoardQuery(
    Guid BoardId) : IRequest<IReadOnlyList<CardDto>>;
