using MediatR;
using WorkBoard.Application.Common.Dtos.BoardMembers;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardMembers;

public class GetBoardMembersQueryHandler
    : IRequestHandler<GetBoardMembersQuery, IReadOnlyList<BoardMemberDto>>
{
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserContext _userContext;

    public GetBoardMembersQueryHandler(
        IBoardMemberRepository boardMemberRepository,
        IUserContext userContext)
    {
        _boardMemberRepository = boardMemberRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<BoardMemberDto>> Handle(
        GetBoardMembersQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        if (userId == null)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        var isMember = await _boardMemberRepository.IsMemberAsync(
            query.BoardId,
            userId.Value,
            cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this board.");
        }

        var membersData = await _boardMemberRepository.GetMembersByBoardAsync(
            query.BoardId, 
            cancellationToken);

        if (membersData == null)
        {
            throw new NotFoundException(
                $"Board with ID {query.BoardId} was not found.");
        }

        return membersData.Select(m => new BoardMemberDto(
            m.User.Id,
            m.User.FullName ?? "Unknown",
            InitialGenerator.Generate(m.User.FullName),
            m.Member.UserRole
        )).ToList().AsReadOnly();
    }
}
