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

namespace WorkBoard.Application.Features.Cards.Commands.CreateCard;

public class CreateCardCommandHandler 
    : IRequestHandler<CreateCardCommand, CardDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public CreateCardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBoardNotificationService notificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<CardDto> Handle(
        CreateCardCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var section = await uow.SectionRepository.GetByIdAsync(
            request.SectionId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {request.SectionId} was not found.");

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            section.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to create cards on this board.");
        }

        var newCard = new Card
        {
            Id = Guid.NewGuid(),
            SectionId = request.SectionId,
            Title = request.Title,
            Position = request.Position,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = newCard.Id,
            UserId = currentUserId,
            Text = ActivityLogMessages.CreatedCard(section.Name),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.CardRepository.CreateAsync(newCard);

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

        var cardDto = _mapper.Map<CardDto>(newCard);

        await _notificationService.SendCardCreatedAsync(
            section.BoardId, 
            cardDto, 
            cancellationToken);

        var logDto = _mapper.Map<ActivityLogDto>(log);
        logDto.FullName = _userContext.FullName!;
        logDto.Initials = InitialGenerator.Generate(_userContext.FullName!);

        await _notificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);

        return cardDto;
    }
}
