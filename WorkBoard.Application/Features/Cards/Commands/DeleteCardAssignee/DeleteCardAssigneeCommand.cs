using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.DeleteCardAssignee;

public class DeleteCardAssigneeCommand : IRequest<Unit>
{
    public Guid CardId { get; set; }
    public Guid TargetUserId { get; init; }
}
