using MediatR;
using WorkBoard.Application.Common.Dtos.Boards;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Boards.Queries.SearchBoards;

public class SearchBoardsQueryHandler
: IRequestHandler<SearchBoardsQuery, IReadOnlyList<BoardSearchResultDto>>
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUserContext _userContext;

    public SearchBoardsQueryHandler(
        IBoardRepository boardRepository,
        IUserContext userContext)
    {
        _boardRepository = boardRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<BoardSearchResultDto>> Handle(
        SearchBoardsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        if (userId == null)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        if (string.IsNullOrWhiteSpace(request.SearchTerm) || 
            request.SearchTerm.Length < 2)
        {
            return new List<BoardSearchResultDto>().AsReadOnly();
        }

        return await _boardRepository.SearchBoardsForUserAsync(
            userId.Value,
            request.SearchTerm,
            cancellationToken);
    }
}
