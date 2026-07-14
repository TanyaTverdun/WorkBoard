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
using static System.Collections.Specialized.BitVector32;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDescription;

public class UpdateCardDescriptionCommandHandler 
    : IRequestHandler<UpdateCardDescriptionCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public UpdateCardDescriptionCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService notificationService,
        IMapper mapper)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(
        UpdateCardDescriptionCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            request.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to edit cards on this board.");
        }

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

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUserId,
            Text = ActivityLogMessages.UpdateCardDescription,
            CreatedAt = DateTime.UtcNow
        };

        card.Description = request.Description;
        card.UpdatedAt = DateTime.UtcNow;
        card.UpdatedBy = currentUserId;

        try
        {
            await uow.CardRepository.UpdateAsync(card, cancellationToken);

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

        var сardDescriptionUpdateDto = new CardDescriptionUpdateDto(
            request.CardId, 
            request.Description);

        await _notificationService.SendCardDescriptionUpdatedAsync(
            section.BoardId,
            сardDescriptionUpdateDto,
            cancellationToken);

        return Unit.Value;
    }
}
