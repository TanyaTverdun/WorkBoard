using MediatR;
using WorkBoard.Application.Common.Dtos.Boards;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardsForArchivation;

public class GetBoardsForArchivationQueryHandler
    : IRequestHandler<GetBoardsForArchivationQuery, IReadOnlyList<BoardArchivationDto>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly IUserContext _userContext;

    public GetBoardsForArchivationQueryHandler(
        IBoardRepository boardRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IUserContext userContext)
    {
        _boardRepository = boardRepository;
        _workspaceMemberRepository = workspaceMemberRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<BoardArchivationDto>> Handle(
        GetBoardsForArchivationQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        return await _boardRepository.GetBoardsForArchivationAsync(
            userId,
            cancellationToken);
    }
}
