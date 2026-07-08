using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Comments;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Comments.Queries.GetCommentsByCard;

public class GetCommentsByCardQueryHandler
    : IRequestHandler<GetCommentsByCardQuery, IReadOnlyList<CommentDto>>
{
    private readonly ICommentRepository _commentRepository;
    private readonly ICardRepository _cardRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetCommentsByCardQueryHandler(
        ICommentRepository commentRepository,
        ICardRepository cardRepository,
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _commentRepository = commentRepository;
        _cardRepository = cardRepository;
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<IReadOnlyList<CommentDto>> Handle(
        GetCommentsByCardQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId
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

        var isBoardMember = await _boardMemberRepository.IsMemberAsync(
            section.BoardId,
            userId,
            cancellationToken);

        if (!isBoardMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this board's comments.");
        }

        var comments = await _commentRepository.GetByCardIdAsync(
            request.CardId,
            cancellationToken);

        var dtos = _mapper.Map<List<CommentDto>>(comments);

        foreach (var dto in dtos)
        {
            dto.Initials = InitialGenerator.Generate(dto.UserFullName);
        }

        return dtos.AsReadOnly();
    }
}
