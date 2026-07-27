using MediatR;
using WorkBoard.Application.Common.Dtos.BoardMembers;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Boards.Queries.GetBoardMembers;

public class GetBoardMembersQueryHandler
    : IRequestHandler<GetBoardMembersQuery, IReadOnlyList<BoardMemberDto>>
{
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserContext _userContext;
    private readonly IBlobStorageService _blobStorageService;

    private const string ContainerName = "avatars";

    public GetBoardMembersQueryHandler(
        IBoardMemberRepository boardMemberRepository,
        IUserContext userContext,
        IBlobStorageService blobStorageService)
    {
        _boardMemberRepository = boardMemberRepository;
        _userContext = userContext;
        _blobStorageService = blobStorageService;
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
            m.User.Email,
            m.User.AvatarUrl = !string.IsNullOrWhiteSpace(m.User.AvatarUrl)
                ? _blobStorageService.GetReadSasUrl(m.User.AvatarUrl, ContainerName)
                : m.User.AvatarUrl,
            m.User.AvatarColor,
            m.Member.UserRole
        )).ToList().AsReadOnly();
    }
}
