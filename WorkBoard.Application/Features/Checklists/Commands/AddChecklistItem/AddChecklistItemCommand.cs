using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;

namespace WorkBoard.Application.Features.Checklists.Commands.AddChecklistItem;

public class AddChecklistItemCommand : IRequest<ChecklistItemDto>
{
    public Guid ChecklistId { get; set; }
    public required string Title { get; set; }
}
