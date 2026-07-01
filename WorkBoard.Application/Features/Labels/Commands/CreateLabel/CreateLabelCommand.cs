using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;

namespace WorkBoard.Application.Features.Labels.Commands.CreateLabel;

public class CreateLabelCommand : IRequest<LabelDto>
{
    public Guid CardId { get; set; }

    public required string Name { get; set; }

    public string? Color { get; set; }
}
