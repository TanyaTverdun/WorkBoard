using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;

namespace WorkBoard.Application.Features.Labels.Queries.GetLabelsByCard;

public class GetLabelsByCardQuery : IRequest<IReadOnlyList<LabelDto>>
{
    public Guid CardId { get; set; }
}
