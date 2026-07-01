using MediatR;

namespace WorkBoard.Application.Features.Labels.Commands.RemoveLabelFromCard;

public class RemoveLabelFromCardCommand : IRequest<Unit>
{
    public Guid CardId { get; set; }

    public Guid LabelId { get; set; }
}
