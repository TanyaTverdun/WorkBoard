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

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardTitle;

public class UpdateCardTitleCommandHandler 
    : IRequestHandler<UpdateCardTitleCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;
    private readonly IMapper _mapper;

    public UpdateCardTitleCommandHandler(
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
        UpdateCardTitleCommand request,
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
            UserId = currentUser.Id,
            Text = ActivityLogMessages.RenamedCard(card.Title, request.Title),
            CreatedAt = DateTime.UtcNow
        };

        card.Title = request.Title;
        card.UpdatedAt = DateTime.UtcNow;
        card.UpdatedBy = currentUser.Id;

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

        var cardRenameDto = new CardRenameDto(
            card.Id,
            card.Title
        );

        await _boardNotificationService.SendCardRenamedAsync(
            request.BoardId,
            cardRenameDto,
            cancellationToken);

        var logDto = _mapper.Map<ActivityLogDto>(log);
        logDto.FullName = currentUser.FullName!;
        logDto.Initials = InitialGenerator.Generate(currentUser.FullName);
        logDto.AvatarUrl = currentUser.AvatarUrl;
        logDto.AvatarColor = currentUser.AvatarColor;

        await _boardNotificationService.SendActivityLogAddedAsync(
            section.BoardId,
            logDto,
            cancellationToken);

        return Unit.Value;
    }
}
