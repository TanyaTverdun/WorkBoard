using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.ActivityLogs.Queries.GetActivityLogsByCard;

public class GetActivityLogsByCardQueryHandler
    : IRequestHandler<GetActivityLogsByCardQuery, IReadOnlyList<ActivityLogDto>>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly ICardRepository _cardRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetActivityLogsByCardQueryHandler(
        IActivityLogRepository activityLogRepository,
        ICardRepository cardRepository,
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _activityLogRepository = activityLogRepository;
        _cardRepository = cardRepository;
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<ActivityLogDto>> Handle(
        GetActivityLogsByCardQuery request,
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
                "You do not have access to this board's activity logs.");
        }

        var logs = await _activityLogRepository.GetByCardIdAsync(
            request.CardId,
            cancellationToken);

        return _mapper.Map<IReadOnlyList<ActivityLogDto>>(logs);
    }
}
