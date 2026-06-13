using MediatR;
using WorkBoard.Application.Common.Dtos.Board;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardsByWorkspace;

public record GetBoardsByWorkspaceQuery(
    Guid WorkspaceId
)
    : IRequest<IReadOnlyList<BoardDto>>;
