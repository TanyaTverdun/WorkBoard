using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Cards.Commands.MoveCard;

public class MoveCardCommandHandler : IRequestHandler<MoveCardCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;
    private readonly IMapper _mapper;


    public MoveCardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService boardNotificationService,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _boardNotificationService = boardNotificationService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(
        MoveCardCommand request, 
        CancellationToken cancellationToken)
    {
        var currentUser = await _userContext.GetCurrentUserFullProfileAsync(
            cancellationToken)
            ?? throw new UnauthorizedAccessException(
                "User profile not found in database.");

        using var uow = _unitOfWorkFactory.Create();

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUser.Id,
            request.BoardId,
            cancellationToken);

        if (membership == null || 
            membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to move cards on this board.");
        }

        var card = await uow.CardRepository.GetByIdAsync(
            request.CardId, 
            cancellationToken);

        if (card == null)
        {
            throw new NotFoundException(
                $"Card with ID {request.CardId} was not found.");
        }

        var targetSection = await uow.SectionRepository.GetByIdAsync(request.NewSectionId, cancellationToken);
        if (targetSection == null || 
            targetSection.BoardId != request.BoardId)
        {
            throw new NotFoundException(
                $"Section with ID {request.NewSectionId} was not found on this board.");
        }

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUser.Id,
            Text = ActivityLogMessages.MovedCard(targetSection.Name),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.CardRepository.UpdateCardPositionAsync(
                request.CardId,
                request.NewSectionId,
                request.NewPosition,
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

        await _boardNotificationService.SendActivityLogAddedAsync(
            targetSection.BoardId,
            logDto,
            cancellationToken);

        var cardMovedDto = new CardMovedDto(
            request.CardId,
            request.NewSectionId,
            targetSection.Name,
            request.NewPosition
        );

        await _boardNotificationService.SendCardMovedAsync(
            request.BoardId,
            cardMovedDto,
            cancellationToken);

        return Unit.Value;
    }
}
