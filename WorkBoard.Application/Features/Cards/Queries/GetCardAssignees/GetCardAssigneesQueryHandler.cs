using MediatR;
using WorkBoard.Application.Common.Dtos.BoardMembers;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Cards.Queries.GetCardAssignees;

public class GetCardAssigneesQueryHandler
: IRequestHandler<GetCardAssigneesQuery, IReadOnlyList<CardAssigneeDto>>
{
    private readonly IUserCardRepository _userCardRepository;
    private readonly ICardRepository _cardRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserContext _userContext;

    public GetCardAssigneesQueryHandler(
        IUserCardRepository userCardRepository,
        ICardRepository cardRepository,
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IUserContext userContext)
    {
        _userCardRepository = userCardRepository;
        _cardRepository = cardRepository;
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<CardAssigneeDto>> Handle(
        GetCardAssigneesQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        if (userId == null)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

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

        var isBoardMember = await _boardMemberRepository.IsMemberAsync(
            section.BoardId,
            userId.Value,
            cancellationToken);

        if (!isBoardMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to view this card's assignees.");
        }

        var assignees = await _userCardRepository.GetAssigneesByCardIdAsync(
            request.CardId,
            cancellationToken);

        return assignees.Select(a => new CardAssigneeDto(
            a.Id,
            a.FullName ?? "Unknown",
            a.Email,
            a.AvatarUrl,
            InitialGenerator.Generate(a.FullName)
        )).ToList().AsReadOnly();
    }
}
