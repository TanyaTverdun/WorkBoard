using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Labels.Queries.GetLabelsByBoard;

public class GetLabelsByBoardQueryHandler
    : IRequestHandler<GetLabelsByBoardQuery, IReadOnlyList<LabelDto>>
{
    private readonly ILabelRepository _labelRepository;
    private readonly IMapper _mapper;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IUserContext _userContext;

    public GetLabelsByBoardQueryHandler(
        ILabelRepository labelRepository,
        IMapper mapper,
        IBoardMemberRepository boardMemberRepository,
        IUserContext userContext)
    {
        _labelRepository = labelRepository;
        _mapper = mapper;
        _boardMemberRepository = boardMemberRepository;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<LabelDto>> Handle(
        GetLabelsByBoardQuery request,
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

        var labels = await _labelRepository.GetByBoardIdAsync(
            request.BoardId,
            cancellationToken);

        return _mapper.Map<IReadOnlyList<LabelDto>>(labels);
    }
}
