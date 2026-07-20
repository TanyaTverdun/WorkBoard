using MediatR;
using WorkBoard.Application.Common.Dtos.Cards;

namespace WorkBoard.Application.Features.Cards.Queries.GetCardDetails;

public record GetCardDetailsQuery(Guid CardId) 
    : IRequest<CardDetailsDto>;
