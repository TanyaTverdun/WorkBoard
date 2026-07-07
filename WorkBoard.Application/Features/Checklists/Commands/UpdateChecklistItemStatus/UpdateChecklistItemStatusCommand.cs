using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklistItemStatus;

public class UpdateChecklistItemStatusCommand : IRequest<ChecklistItemDto>
{
    public Guid ItemId { get; set; }
    public bool IsDone { get; set; }
}
