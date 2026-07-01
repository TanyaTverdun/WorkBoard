using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;

namespace WorkBoard.Application.Features.Labels.Queries.GetLabelsByBoard;

public class GetLabelsByBoardQuery : IRequest<IReadOnlyList<LabelDto>>
{
    public Guid BoardId { get; set; }
}
