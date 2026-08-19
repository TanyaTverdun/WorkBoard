using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Cards.Commands.AddCardAssignee;

public class AddCardAssigneeCommandHandler
    : IRequestHandler<AddCardAssigneeCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;
    private readonly IBlobStorageService _blobStorageService;

    public AddCardAssigneeCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBoardNotificationService notificationService,
        IBlobStorageService blobStorageService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _notificationService = notificationService;
        _blobStorageService = blobStorageService;
    }

    public async Task<Unit> Handle(
        AddCardAssigneeCommand request,
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
                "You do not have access to modify this card.");
        }

        var isTargetBoardMember = await uow.BoardMemberRepository.IsMemberAsync(
            section.BoardId,
            request.TargetUserId,
            cancellationToken);

        if (!isTargetBoardMember)
        {
            throw new InvalidOperationException(
                $"User {request.TargetUserId} cannot be assigned " +
                $"because he is not a member of this board.");
        }

        var isAlreadyAssigned = await uow.UserCardRepository.IsAssignedAsync(
            request.CardId,
            request.TargetUserId,
            cancellationToken);

        if (isAlreadyAssigned)
        {
            throw new InvalidOperationException(
                $"User {request.TargetUserId} is already assigned to this card.");
        }

        var user = await uow.UserRepository.GetByIdAsync(
            request.TargetUserId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"User with ID {request.TargetUserId} was not found.");

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            UserId = currentUser.Id,
            Text = ActivityLogMessages.AddedAssignee(user.FullName!),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.UserCardRepository.AddAssigneeAsync(
                request.CardId,
                request.TargetUserId,
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
        logDto.FullName = currentUser.FullName!;
        logDto.Initials = InitialGenerator.Generate(currentUser.FullName);
        logDto.AvatarUrl = currentUser.AvatarUrl;
        logDto.AvatarColor = currentUser.AvatarColor;

        await _notificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);

        var avatarUrl = user.AvatarUrl;
        if (!string.IsNullOrWhiteSpace(avatarUrl))
        {
            avatarUrl = _blobStorageService.GetReadSasUrl(
                avatarUrl,
                BlobContainers.Avatars);
        }

        var assigneeDto = new CardAssigneeDto
        {
            UserId = user.Id,
            FullName = user.FullName ?? "Unknown",
            Email = user.Email,
            AvatarUrl = avatarUrl,
            AvatarColor = user.AvatarColor,
            Initials = InitialGenerator.Generate(user.FullName)
        };

        var assigneeAddDto = new AssigneeAddDto(request.CardId, assigneeDto);

        await _notificationService.SendAssigneeAddedAsync(
            section.BoardId,
            assigneeAddDto,
            cancellationToken);

        return Unit.Value;
    }
}
