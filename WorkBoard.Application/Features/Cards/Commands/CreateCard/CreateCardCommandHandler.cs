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
        var currentUser = await _userContext.GetCurrentUserFullProfileAsync(
            cancellationToken)
            ?? throw new UnauthorizedAccessException(
                "User profile not found in database.");

        using var uow = _unitOfWorkFactory.Create();

        var section = await uow.SectionRepository.GetByIdAsync(
            request.SectionId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Section with ID {request.SectionId} was not found.");

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUser.Id,
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
            CreatedBy = currentUser.Id
        };

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = newCard.Id,
            UserId = currentUser.Id,
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
        logDto.FullName = currentUser.FullName!;
        logDto.Initials = InitialGenerator.Generate(currentUser.FullName);
        logDto.AvatarUrl = currentUser.AvatarUrl;
        logDto.AvatarColor = currentUser.AvatarColor;

        await _notificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);

        return cardDto;
    }
}
