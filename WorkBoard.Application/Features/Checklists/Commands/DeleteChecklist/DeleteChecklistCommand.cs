using MediatR;

namespace WorkBoard.Application.Features.Checklists.Commands.DeleteChecklist;

public class DeleteChecklistCommand : IRequest
{
    public Guid ChecklistId { get; set; }
}
