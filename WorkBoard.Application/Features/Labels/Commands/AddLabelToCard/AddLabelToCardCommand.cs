using MediatR;

namespace WorkBoard.Application.Features.Labels.Commands.AddLabelToCard;

public class AddLabelToCardCommand : IRequest<Unit>
{
    public Guid CardId { get; set; }

    public Guid LabelId { get; set; }
}
