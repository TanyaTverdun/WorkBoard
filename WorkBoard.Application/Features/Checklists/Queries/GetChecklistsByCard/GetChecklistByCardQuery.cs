using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;

namespace WorkBoard.Application.Features.Checklists.Queries.GetChecklistsByCard;

public record GetChecklistByCardQuery(Guid CardId) 
    : IRequest<ChecklistDto?>;
