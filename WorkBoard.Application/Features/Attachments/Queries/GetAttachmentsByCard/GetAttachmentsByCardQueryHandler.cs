using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.Attachments;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Repositories;

namespace WorkBoard.Application.Features.Attachments.Queries.GetAttachmentsByCard;

public class GetAttachmentsByCardQueryHandler
: IRequestHandler<GetAttachmentsByCardQuery, IReadOnlyList<AttachmentDto>>
{
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly ICardRepository _cardRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IBlobStorageService _blobStorageService;

    public GetAttachmentsByCardQueryHandler(
        IAttachmentRepository attachmentRepository,
        ICardRepository cardRepository,
        ISectionRepository sectionRepository,
        IBoardMemberRepository boardMemberRepository,
        IMapper mapper,
        IUserContext userContext,
        IBlobStorageService blobStorageService)
    {
        _attachmentRepository = attachmentRepository;
        _cardRepository = cardRepository;
        _sectionRepository = sectionRepository;
        _boardMemberRepository = boardMemberRepository;
        _mapper = mapper;
        _userContext = userContext;
        _blobStorageService = blobStorageService;
    }

    public async Task<IReadOnlyList<AttachmentDto>> Handle(
        GetAttachmentsByCardQuery request,
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
                "You do not have access to this board's attachments.");
        }

        var attachments = await _attachmentRepository.GetByCardIdAsync(
            request.CardId,
            cancellationToken);

        var dtos = _mapper.Map<IReadOnlyList<AttachmentDto>>(attachments);

        foreach (var dto in dtos)
        {
            dto.FileUrl = _blobStorageService.GetReadSasUrl(
                dto.FileUrl,
                BlobContainers.Attachments);
        }

        return dtos;
    }
}
