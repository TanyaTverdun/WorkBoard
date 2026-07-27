using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Attachments;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Attachments.Commands.AddAttachment;

public class AddAttachmentCommandHandler
    : IRequestHandler<AddAttachmentCommand, AttachmentDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IBoardNotificationService _notificationService;

    public AddAttachmentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBlobStorageService blobStorageService,
        IBoardNotificationService notificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
        _notificationService = notificationService;
    }

    public async Task<AttachmentDto> Handle(
        AddAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = await _userContext.GetCurrentUserFullProfileAsync(
            cancellationToken)
            ?? throw new UnauthorizedAccessException(
                "User profile not found in database.");

        using var uow = _unitOfWorkFactory.Create();

        var card = await uow.CardRepository.GetByIdAsync(
            request.CardId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {request.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId, 
            currentUser.Id, 
            cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to add attachments to this card.");
        }

        var uploadedFileUrl = await _blobStorageService.UploadAsync(
            request.FileStream,
            request.FileName,
            BlobContainers.Attachments,
            request.ContentType,
            cancellationToken);

        var attachment = new Attachment
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            FileUrl = uploadedFileUrl,
            FileName = request.FileName,
            FileSizeBytes = request.FileSizeBytes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser.Id
        };

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            UserId = currentUser.Id,
            Text = ActivityLogMessages.AttachedFile(request.FileName),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.AttachmentRepository.CreateAsync(
                attachment, 
                cancellationToken);

            await uow.ActivityLogRepository.CreateAsync(
                log, 
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        var dto = _mapper.Map<AttachmentDto>(attachment);

        dto.FileUrl = _blobStorageService.GetReadSasUrl(
                dto.FileUrl,
                BlobContainers.Attachments);

        var attachmentAddedDto = new AttachmentAddedDto(request.CardId, dto);

        await _notificationService.SendAttachmentAddedAsync(
            section.BoardId,
            attachmentAddedDto,
            cancellationToken);

        var logDto = _mapper.Map<ActivityLogDto>(log);
        logDto.FullName = currentUser.FullName!;
        logDto.Initials = InitialGenerator.Generate(currentUser.FullName);
        logDto.AvatarUrl = currentUser.AvatarUrl;
        logDto.AvatarColor = currentUser.AvatarColor;

        await _notificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);

        return dto;
    }
}
