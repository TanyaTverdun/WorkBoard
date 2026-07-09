using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Labels.Commands.AddLabelToCard;

public class AddLabelToCardCommandHandler
: IRequestHandler<AddLabelToCardCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public AddLabelToCardCommandHandler(
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
        AddLabelToCardCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

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

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            section.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to modify labels on this card.");
        }

        var label = await uow.LabelRepository.GetByIdAsync(
            request.LabelId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Label with ID {request.LabelId} was not found.");

        if (label.BoardId != section.BoardId)
        {
            throw new InvalidOperationException(
                "Cannot attach a label from a different board.");
        }

        var isAlreadyAttached = await uow.CardLabelRepository.HasLabelAsync(
            request.CardId,
            request.LabelId,
            cancellationToken);

        if (isAlreadyAttached)
        {
            return Unit.Value;
        }

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUserId,
            Text = ActivityLogMessages.AddedLabelToCard(label.Name),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.CardLabelRepository.AddAsync(
                request.CardId,
                request.LabelId,
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

        return Unit.Value;
    }
}
