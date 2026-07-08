using AutoMapper;
using MediatR;
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
                "You do not have access to add comments to this card.");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            UserId = currentUserId,
            Text = request.Text,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.CommentRepository.CreateAsync(comment, cancellationToken);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        comment.UserFullName = _userContext.FullName;

        var dto = _mapper.Map<CommentDto>(comment);

        dto.Initials = InitialGenerator.Generate(dto.UserFullName);

        await _notificationService.SendCommentAddedAsync(
            section.BoardId,
            dto,
            cancellationToken);

        return dto;
    }
}
