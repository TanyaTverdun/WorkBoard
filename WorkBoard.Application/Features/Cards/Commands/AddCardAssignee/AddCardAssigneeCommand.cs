using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.AddCardAssignee;

public record AddCardAssigneeCommand : IRequest<Unit>
{
    public Guid CardId { get; init; }
    public Guid TargetUserId { get; init; }
}
