using MediatR;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Notification;

namespace WorkBoard.Application.Features.User.Commands.UpdateAvatarImage;

public class UpdateUserAvatarImageCommandHandler
    : IRequestHandler<UpdateUserAvatarImageCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _notificationService;
    private readonly IBlobStorageService _blobStorageService;

    private const string ContainerName = "avatars";

    public UpdateUserAvatarImageCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService notificationService,
        IBlobStorageService blobStorageService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _notificationService = notificationService;
        _blobStorageService = blobStorageService;
    }

    public async Task Handle(
        UpdateUserAvatarImageCommand request,
        CancellationToken cancellationToken)
    {
        var currentUser = await _userContext.GetCurrentUserFullProfileAsync(
            cancellationToken)
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        var rawAvatarUrl = await _blobStorageService.UploadAsync(
            request.FileStream,
            request.FileName,
            ContainerName,
            request.ContentType,
            cancellationToken);

        using var uow = _unitOfWorkFactory.Create();

        try
        {
            var affectedRows = await uow.UserRepository.UpdateAvatarUrlAsync(
                currentUser.Id,
                rawAvatarUrl,
                cancellationToken);

            if (affectedRows == 0)
            {
                throw new NotFoundException(
                    $"User with ID {currentUser.Id} was not found.");
            }

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            await _blobStorageService.DeleteAsync(
                rawAvatarUrl, 
                ContainerName, 
                cancellationToken);
            throw;
        }

        if (!string.IsNullOrEmpty(currentUser.AvatarUrl))
        {
            await _blobStorageService.DeleteAsync(
                    currentUser.AvatarUrl,
                    ContainerName,
                    cancellationToken);
        }

        var sasAvatarUrl = _blobStorageService.GetReadSasUrl(
            rawAvatarUrl,
            ContainerName);

        var userBoardIds = await uow.BoardMemberRepository.GetBoardIdsByUserIdAsync(
                currentUser.Id,
                cancellationToken);

        var notificationData = new UserAvatarUpdatedDto
        {
            UserId = currentUser.Id,
            AvatarUrl = sasAvatarUrl,
            AvatarColor = null
        };

        var notificationTasks = userBoardIds.Select(boardId =>
            _notificationService.SendUserAvatarUpdatedAsync(
                boardId,
                notificationData,
                cancellationToken));

        await Task.WhenAll(notificationTasks);
    }
}
