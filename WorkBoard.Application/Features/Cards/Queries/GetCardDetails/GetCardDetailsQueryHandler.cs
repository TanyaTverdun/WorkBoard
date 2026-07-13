using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Attachments;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Dtos.Comments;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Cards.Queries.GetCardDetails;

public class GetCardDetailsQueryHandler
    : IRequestHandler<GetCardDetailsQuery, CardDetailsDto>
{
    private readonly ICardRepository _cardRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetCardDetailsQueryHandler(
        ICardRepository cardRepository,
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IBlobStorageService blobStorageService,
        IMapper mapper,
        IUserContext userContext)
    {
        _cardRepository = cardRepository;
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _blobStorageService = blobStorageService;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<CardDetailsDto> Handle(
        GetCardDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        var fullData = await _cardRepository.GetCardFullDataAsync(
            request.CardId, cancellationToken)
            ?? throw new NotFoundException(
                $"Card with ID {request.CardId} was not found.");

        var section = await _sectionRepository.GetByIdAsync(
            fullData.Card.SectionId, 
            cancellationToken)
            ?? throw new NotFoundException(
                $"Section with ID {fullData.Card.SectionId} was not found.");

        var isBoardMember = await _boardMemberRepository.IsMemberAsync(
            section.BoardId,
            userId,
            cancellationToken);

        if (!isBoardMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this card.");
        }

        var assigneesDtos = fullData.Assignees.Select(a => new CardAssigneeDto(
            a.Id,
            a.FullName ?? "Unknown",
            a.Email,
            a.AvatarUrl,
            InitialGenerator.Generate(a.FullName)
        )).ToList().AsReadOnly();

        var attachmentsDtos = _mapper.Map<List<AttachmentDto>>(
            fullData.Attachments);

        foreach (var dto in attachmentsDtos)
        {
            dto.FileUrl = _blobStorageService.GetReadSasUrl(
                dto.FileUrl, 
                BlobContainers.Attachments);
        }

        var commentsDtos = _mapper.Map<List<CommentDto>>(
            fullData.Comments);

        foreach (var dto in commentsDtos)
        {
            dto.Initials = InitialGenerator.Generate(dto.UserFullName);
        }

        var activityLogsDtos = _mapper.Map<List<ActivityLogDto>>(
            fullData.ActivityLogs);

        foreach (var dto in activityLogsDtos)
        {
            dto.Initials = InitialGenerator.Generate(dto.FullName);
        }

        var result = new CardDetailsDto
        {
            Id = fullData.Card.Id,
            SectionId = fullData.Card.SectionId,
            Title = fullData.Card.Title,
            Description = fullData.Card.Description,
            DueDate = fullData.Card.DueDate?.Date,
            Position = fullData.Card.Position,

            Assignees = assigneesDtos,
            Labels = _mapper.Map<IReadOnlyList<LabelDto>>(fullData.Labels),
            Checklist = _mapper.Map<ChecklistDto>(fullData.Checklist),
            Attachments = attachmentsDtos.AsReadOnly(),
            Comments = commentsDtos.AsReadOnly(),
            ActivityLogs = activityLogsDtos
        };

        return result;
    }
}
