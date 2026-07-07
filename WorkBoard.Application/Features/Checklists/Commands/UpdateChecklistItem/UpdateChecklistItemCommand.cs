using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklistItem;

public class UpdateChecklistItemCommand : IRequest<ChecklistItemDto>
{
    public Guid ItemId { get; set; }
    public required string Title { get; set; }
}
