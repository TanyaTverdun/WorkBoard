using MediatR;
using WorkBoard.Application.Common.Dtos.Checklists;

namespace WorkBoard.Application.Features.Checklists.Commands.CreateChecklist;

public class CreateChecklistCommand : IRequest<ChecklistDto>
{
    public Guid CardId { get; set; }
    public string Name { get; set; } = string.Empty;
}
