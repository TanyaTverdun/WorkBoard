using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Sections.Commands.CreateSection;

public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;
    private readonly IMapper _mapper;

    public CreateSectionCommandHandler(
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

    public async Task<Guid> Handle(
        CreateSectionCommand request,
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
                "You do not have permission to create sections on this board.");
        }

        var existingSections = await uow.SectionRepository.GetByBoardIdAsync(
            request.BoardId,
            currentUserId,
            cancellationToken);

        double nextPosition = existingSections.Count > 0
            ? existingSections.Max(s => s.Position) + 1.0
            : 1.0;

        var sectionId = Guid.NewGuid();

        var section = new Section
        {
            Id = sectionId,
            BoardId = request.BoardId,
            Name = request.Name,
            Position = nextPosition,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        try
        {
            await uow.SectionRepository.CreateAsync(
                section,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        var sectionDto = _mapper.Map<SectionDto>(section);

        await _boardNotificationService.SendSectionCreatedAsync(
            request.BoardId,
            sectionDto,
            cancellationToken);

        return sectionId;
    }
}