using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Labels.Commands.UpdateLabel;

public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, LabelDto>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _notificationService;

    public UpdateLabelCommandHandler(
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

    public async Task<LabelDto> Handle(
        UpdateLabelCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var label = await uow.LabelRepository.GetByIdAsync(
            request.LabelId,
            cancellationToken)
                ?? throw new NotFoundException($"Label with ID {request.LabelId} was not found.");

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            label.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to modify labels on this board.");
        }

        if (label.Name != request.Name)
        {
            var isNameUnique = await uow.LabelRepository.IsNameUniqueAsync(
                label.BoardId,
                request.Name,
                cancellationToken);

            if (!isNameUnique)
            {
                throw new InvalidOperationException(
                    $"Label with name '{request.Name}' already exists on this board.");
            }
        }

        label.Name = request.Name;
        label.Color = request.Color;
        label.UpdatedAt = DateTime.UtcNow;
        label.UpdatedBy = currentUserId;

        try
        {
            await uow.LabelRepository.UpdateAsync(label, cancellationToken);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        var labelDto = _mapper.Map<LabelDto>(label);

        await _notificationService.SendLabelUpdatedAsync(
            label.BoardId, 
            labelDto, 
            cancellationToken);

        return labelDto;
    }
}
