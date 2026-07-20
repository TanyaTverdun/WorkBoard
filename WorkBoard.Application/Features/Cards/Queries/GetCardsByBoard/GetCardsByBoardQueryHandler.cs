using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Cards.Queries.GetCardsByBoard;

public class GetCardsByBoardQueryHandler
: IRequestHandler<GetCardsByBoardQuery, IReadOnlyList<CardDto>>
{
    private readonly ICardRepository _cardRepository;
    private readonly IMapper _mapper;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserContext _userContext;

    public GetCardsByBoardQueryHandler(
        ICardRepository cardRepository,
        IMapper mapper,
        IBoardMemberRepository boardMemberRepository,
        IUserContext userContext)
    {
        _cardRepository = cardRepository;
        _mapper = mapper;
        _boardMemberRepository = boardMemberRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<CardDto>> Handle(
        GetCardsByBoardQuery request,
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

        var rawCards = await _cardRepository.GetCardsByBoardIdAsync(
            request.BoardId,
            cancellationToken);

        var cardDtos = _mapper.Map<List<CardDto>>(rawCards);

        foreach (var card in cardDtos)
        {
            foreach (var assignee in card.Assignees)
            {
                assignee.Initials = InitialGenerator.Generate(assignee.FullName);
            }
        }

        return cardDtos.AsReadOnly();
    }
}
