using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Labels.Commands.DeleteLabel;

public class DeleteLabelCommandHandler 
    : IRequestHandler<DeleteLabelCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _notificationService;

    public DeleteLabelCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService notificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(
        DeleteLabelCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var label = await uow.LabelRepository.GetByIdAsync(
            request.LabelId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Label with ID {request.LabelId} was not found.");

        var membership = await uow.BoardMemberRepository.GetMembershipAsync(
            currentUserId,
            label.BoardId,
            cancellationToken);

        if (membership == null || membership.UserRole == BoardRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to delete labels on this board.");
        }

        try
        {
            await uow.CardLabelRepository.RemoveAllByLabelIdAsync(
                request.LabelId,
                cancellationToken);

            await uow.LabelRepository.DeleteAsync(
                label.Id,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        await _notificationService.SendLabelDeletedAsync(
            label.BoardId, 
            request.LabelId, 
            cancellationToken);

        return Unit.Value;
    }
}
