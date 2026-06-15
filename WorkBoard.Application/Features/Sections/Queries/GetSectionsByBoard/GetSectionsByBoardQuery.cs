using MediatR;
using WorkBoard.Application.Common.Dtos.Section;

namespace WorkBoard.Application.Features.Sections.Queries.GetSectionsByBoard;

public record GetSectionsByBoardQuery(
    Guid BoardId) : IRequest<IReadOnlyList<SectionDto>>;