using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Labels.Queries.GetLabelsByCard;

public class GetLabelsByCardQueryHandler
: IRequestHandler<GetLabelsByCardQuery, IReadOnlyList<LabelDto>>
{
    private readonly ILabelRepository _labelRepository;
    private readonly ICardRepository _cardRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetLabelsByCardQueryHandler(
        ILabelRepository labelRepository,
        ICardRepository cardRepository,
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _labelRepository = labelRepository;
        _cardRepository = cardRepository;
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<LabelDto>> Handle(
        GetLabelsByCardQuery request,
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
                "You do not have access to this board's labels.");
        }

        var labels = await _labelRepository.GetByCardIdAsync(
            request.CardId,
            cancellationToken);

        return _mapper.Map<IReadOnlyList<LabelDto>>(labels);
    }
}
