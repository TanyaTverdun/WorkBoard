using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Attachments.Commands.DeleteAttachment;

public class DeleteAttachmentCommandHandler 
    : IRequestHandler<DeleteAttachmentCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IBoardNotificationService _notificationService;
    private readonly IMapper _mapper;

    public DeleteAttachmentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBlobStorageService blobStorageService,
        IBoardNotificationService notificationService,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _blobStorageService = blobStorageService;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task Handle(
        DeleteAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var attachment = await uow.AttachmentRepository.GetByIdAsync(
            request.AttachmentId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Attachment with ID {request.AttachmentId} was not found.");

        if (attachment.CardId != request.CardId)
        {
            throw new ForbiddenAccessException(
                "This attachment does not belong to the specified card.");
        }

        var card = await uow.CardRepository.GetByIdAsync(
            attachment.CardId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Card with ID {attachment.CardId} was not found.");

        var section = await uow.SectionRepository.GetByIdAsync(
            card.SectionId, cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {card.SectionId} was not found.");

        var isCurrentMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId, currentUserId, cancellationToken);

        if (!isCurrentMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to delete attachments from this card.");
        }

        await _blobStorageService.DeleteAsync(
            attachment.FileUrl,
            BlobContainers.Attachments,
            cancellationToken);

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            UserId = currentUserId,
            Text = ActivityLogMessages.DeletedAttachment(attachment.FileName),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.AttachmentRepository.DeleteAsync(
                request.AttachmentId,
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

        var logDto = _mapper.Map<ActivityLogDto>(log);
        logDto.FullName = _userContext.FullName!;
        logDto.Initials = InitialGenerator.Generate(_userContext.FullName!);

        await _notificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);
    }
}
