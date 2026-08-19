using MediatR;
using WorkBoard.Application.Common.Dtos.Boards;

namespace WorkBoard.Application.Features.Boards.Queries.SearchBoards;

public record SearchBoardsQuery(string SearchTerm)
    : IRequest<IReadOnlyList<BoardSearchResultDto>>;
