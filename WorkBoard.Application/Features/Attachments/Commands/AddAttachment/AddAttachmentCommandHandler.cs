using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Attachments;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Attachments.Commands.AddAttachment;

public class AddAttachmentCommandHandler
    : IRequestHandler<AddAttachmentCommand, AttachmentDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;

    public AddAttachmentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBlobStorageService blobStorageService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }

    public async Task<AttachmentDto> Handle(
        AddAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var card = await uow.CardRepository.GetByIdAsync(
            request.CardId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {request.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId, currentUserId, cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to add attachments to this card.");
        }

        var uploadedFileUrl = await _blobStorageService.UploadAsync(
            request.FileStream,
            request.FileName,
            "attachments",
            request.ContentType,
            cancellationToken);

        var attachment = new Attachment
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            FileUrl = uploadedFileUrl,
            FileName = request.FileName,
            FileSizeBytes = request.FileSizeBytes
        };

        try
        {
            await uow.AttachmentRepository.CreateAsync(
                attachment, 
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return _mapper.Map<AttachmentDto>(attachment);
    }
}
