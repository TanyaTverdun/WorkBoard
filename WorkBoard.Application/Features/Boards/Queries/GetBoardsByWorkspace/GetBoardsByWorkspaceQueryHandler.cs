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
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public GetBoardsByWorkspaceQueryHandler(
        IBoardRepository boardRepository,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _boardRepository = boardRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<BoardDto>> Handle(
        GetBoardsByWorkspaceQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var isMember = await uow.WorkspaceMemberRepository.IsMemberAsync(
            request.WorkspaceId,
            currentUserId,
            cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException(
                "You don't have access to this workspace.");
        }

        return await _boardRepository.GetByWorkspaceIdAsync(
            request.WorkspaceId,
            cancellationToken);
    }
}
