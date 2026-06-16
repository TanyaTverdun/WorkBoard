using MediatR;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Sections.Queries.GetSectionsByBoard;

public class GetSectionsByBoardQueryHandler
    : IRequestHandler<GetSectionsByBoardQuery, IReadOnlyList<SectionDto>>
{
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserContext _userContext;

    public GetSectionsByBoardQueryHandler(
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IUserContext userContext)
    {
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<SectionDto>> Handle(
        GetSectionsByBoardQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        if (userId == null)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        var isBoardMember = await _boardMemberRepository.IsMemberAsync(
            request.BoardId,
            userId.Value,
            cancellationToken);

        if (!isBoardMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this board.");
        }

        return await _sectionRepository.GetByBoardIdAsync(
            request.BoardId,
            userId.Value,
            cancellationToken);
    }
}
