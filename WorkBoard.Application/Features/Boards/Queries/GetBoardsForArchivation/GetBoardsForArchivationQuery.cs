using MediatR;
using WorkBoard.Application.Common.Dtos.Boards;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardsForArchivation;

public record GetBoardsForArchivationQuery()
    : IRequest<IReadOnlyList<BoardArchivationDto>>;
