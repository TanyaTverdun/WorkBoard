using MediatR;
using WorkBoard.Application.Common.Checklists;

namespace WorkBoard.Application.Features.Checklists.Queries.GetChecklistsByCard;

public record GetChecklistsByCardQuery(Guid CardId) 
    : IRequest<IReadOnlyList<ChecklistItemDto>>;
