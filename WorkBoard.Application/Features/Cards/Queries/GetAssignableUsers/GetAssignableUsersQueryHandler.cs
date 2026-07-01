using MediatR;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Cards.Queries.GetAssignableUsers;

public class GetAssignableUsersQueryHandler
: IRequestHandler<GetAssignableUsersQuery, IReadOnlyList<UserSearchDto>>
{
    private readonly ICardRepository _cardRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserCardRepository _userCardRepository;
    private readonly IUserContext _userContext;

    public GetAssignableUsersQueryHandler(
        ICardRepository cardRepository,
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IUserCardRepository userCardRepository,
        IUserContext userContext)
    {
        _cardRepository = cardRepository;
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _userCardRepository = userCardRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<UserSearchDto>> Handle(
        GetAssignableUsersQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        var card = await _cardRepository.GetByIdAsync(
            request.CardId, 
            cancellationToken)
            ?? throw new NotFoundException(
                $"Card with ID {request.CardId} was not found.");

        var section = await _sectionRepository.GetByIdAsync(
            card.SectionId, 
            cancellationToken)
            ?? throw new NotFoundException(
                $"Section with ID {card.SectionId} was not found.");

        var isMember = await _boardMemberRepository.IsMemberAsync(
            section.BoardId,
            currentUserId,
            cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this board.");
        }

        var usersFromDb = await _userCardRepository.GetAssignableUsersAsync(
            section.BoardId,
            request.CardId,
            cancellationToken);

        foreach (var user in usersFromDb)
        {
            user.Initials = InitialGenerator.Generate(user.FullName);
        }

        return usersFromDb;
    }
}
