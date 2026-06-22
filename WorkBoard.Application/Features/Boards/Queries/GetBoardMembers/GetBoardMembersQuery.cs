using MediatR;
using WorkBoard.Application.Common.Dtos.BoardMembers;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardMembers;

public record GetBoardMembersQuery(
    Guid BoardId) : IRequest<IReadOnlyList<BoardMemberDto>>;
