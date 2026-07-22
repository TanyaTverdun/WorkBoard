using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Comments;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Comments.Commands.CreateComment;

public class CreateCommentCommandHandler
    : IRequestHandler<CreateCommentCommand, CommentDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public CreateCommentCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBoardNotificationService boardNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _notificationService = boardNotificationService;
    }

    public async Task<CommentDto> Handle(
        CreateCommentCommand request,
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
                "You do not have access to add comments to this card.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            UserId = currentUser.Id,
            Text = request.Text,
            CreatedAt = DateTime.UtcNow
        };

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUser.Id,
            Text = ActivityLogMessages.CreatedComment,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.CommentRepository.CreateAsync(
                comment, 
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

        var commentDto = _mapper.Map<CommentDto>(comment);

        commentDto.UserFullName = currentUser.FullName!;
        commentDto.UserAvatarUrl = currentUser.AvatarUrl;
        commentDto.UserAvatarColor = currentUser.AvatarColor;
        commentDto.Initials = InitialGenerator.Generate(commentDto.UserFullName);

        await _notificationService.SendCommentAddedAsync(
            section.BoardId,
            commentDto,
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

        return commentDto;
    }
}
