using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;

namespace WorkBoard.Application.Features.Checklists.Commands.UpdateChecklist;

public class UpdateChecklistCommand : IRequest<ChecklistDto>
{
    public Guid ChecklistId { get; set; }
    public string Name { get; set; } = string.Empty;
}
