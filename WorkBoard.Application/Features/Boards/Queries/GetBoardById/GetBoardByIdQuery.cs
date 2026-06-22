using MediatR;
using WorkBoard.Application.Common.Dtos.Board;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardById;

public record GetBoardByIdQuery(
    Guid BoardId) : IRequest<BoardDto>;
