using MediatR;
using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardsByWorkspace;

public class GetBoardsByWorkspaceQueryHandler
: IRequestHandler<GetBoardsByWorkspaceQuery, IReadOnlyList<BoardDto>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IWorkspaceMemberRepository _workspaceMemberRepository;
    private readonly IUserContext _userContext;

    public GetBoardsByWorkspaceQueryHandler(
        IBoardRepository boardRepository,
        IWorkspaceMemberRepository workspaceMemberRepository,
        IUserContext userContext)
    {
        _boardRepository = boardRepository;
        _workspaceMemberRepository = workspaceMemberRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<BoardDto>> Handle(
        GetBoardsByWorkspaceQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        if (userId == null)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        var isWorkspaceMember = await _workspaceMemberRepository.IsMemberAsync(
        request.WorkspaceId,
        userId.Value,
        cancellationToken);

        if (!isWorkspaceMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this workspace.");
        }

        return await _boardRepository.GetByWorkspaceIdAsync(
            request.WorkspaceId,
            userId.Value,
            cancellationToken);
    }
}
