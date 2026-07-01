using MediatR;

namespace WorkBoard.Application.Features.Labels.Commands.DeleteLabel;

public class DeleteLabelCommand : IRequest<Unit>
{
    public Guid LabelId { get; set; }
}
