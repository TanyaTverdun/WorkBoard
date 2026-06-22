using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardById;

public class GetBoardByIdQueryHandler 
    : IRequestHandler<GetBoardByIdQuery, BoardDto>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IBoardMemberRepository _memberRepository;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetBoardByIdQueryHandler(
        IBoardRepository boardRepository,
        IBoardMemberRepository memberRepository,
        IUserContext userContext,
        IMapper mapper)
    {
        _boardRepository = boardRepository;
        _memberRepository = memberRepository;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<BoardDto> Handle(
        GetBoardByIdQuery query,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        if (userId == null)
        {
            throw new UnauthorizedAccessException(
                 "User is not authenticated.");
        }

        var isMember = await _memberRepository.IsMemberAsync(
            query.BoardId,
            userId.Value,
            cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this board.");
        }

        var board = await _boardRepository.GetByIdAsync(
            query.BoardId, 
            cancellationToken);

        if (board == null)
        {
            throw new NotFoundException(
                $"Board with ID {query.BoardId} was not found.");
        }

        return _mapper.Map<BoardDto>(board);
    }
}
