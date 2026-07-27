using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.ActivityLogs;
using WorkBoard.Application.Common.Dtos.Checklists;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Features.Checklists.Commands.CreateChecklist;

public class CreateChecklistCommandHandler
    : IRequestHandler<CreateChecklistCommand, ChecklistDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public CreateChecklistCommandHandler(
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

    public async Task<ChecklistDto> Handle(
        CreateChecklistCommand request,
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

        var checklist = new Checklist
        {
            Id = Guid.NewGuid(),
            CardId = request.CardId,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUser.Id
        };

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            UserId = currentUser.Id,
            Text = ActivityLogMessages.CreatedChecklist(request.Name),
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await uow.ChecklistRepository.CreateAsync(
                checklist, 
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

        var checklistDto = _mapper.Map<ChecklistDto>(checklist);

        var checklistCreatedDto = new ChecklistCreatedDto(
            request.CardId, 
            checklistDto);

        await _notificationService.SendChecklistCreatedAsync(
            section.BoardId,
            checklistCreatedDto,
            cancellationToken);

        return checklistDto;
    }
}
