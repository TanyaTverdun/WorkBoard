using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;

namespace WorkBoard.Application.Features.Labels.Commands.UpdateLabel;

public class UpdateLabelCommand : IRequest<LabelDto>
{
    public Guid LabelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
