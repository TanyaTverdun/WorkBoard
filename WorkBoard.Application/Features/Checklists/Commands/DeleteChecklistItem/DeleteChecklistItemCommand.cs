using MediatR;

namespace WorkBoard.Application.Features.Checklists.Commands.DeleteChecklistItem;

public class DeleteChecklistItemCommand : IRequest
{
    public Guid ItemId { get; set; }
}
