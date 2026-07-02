using MediatR;
using WorkBoard.Application.Common.Dtos.Cards;

namespace WorkBoard.Application.Features.Cards.Queries.GetCardAssignees;

public class GetCardAssigneesQuery 
    : IRequest<IReadOnlyList<CardAssigneeDto>>
{
    public Guid CardId { get; set; }
}
