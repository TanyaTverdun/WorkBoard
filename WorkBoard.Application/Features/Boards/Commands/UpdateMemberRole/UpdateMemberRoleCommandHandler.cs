using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.UpdateMemberRole;

public class UpdateMemberRoleCommandHandler
    : IRequestHandler<UpdateMemberRoleCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;

    public UpdateMemberRoleCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService boardNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _boardNotificationService = boardNotificationService;
    }

    public async Task<Unit> Handle(
        UpdateMemberRoleCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(
            request.BoardId,
            cancellationToken)
                ?? throw new NotFoundException(
                    $"Board with ID {request.BoardId} was not found.");

        var spaceMembership = await uow.WorkspaceMemberRepository.GetMembershipAsync(
            currentUserId,
            board.WorkspaceId,
            cancellationToken);

        if (spaceMembership == null ||
            spaceMembership.UserRole == WorkspaceRole.Observer)
        {
            throw new ForbiddenAccessException(
                "Observers in the space cannot change board roles.");
        }

        var isTargetUserMember = await uow.BoardMemberRepository.IsMemberAsync(
            request.BoardId,
            request.TargetUserId,
            cancellationToken);

        if (!isTargetUserMember)
        {
            throw new NotFoundException(
                $"User {request.TargetUserId} is not a member of this board.");
        }

        try
        {
            var rowsAffected = await uow.BoardMemberRepository.UpdateRoleAsync(
                request.BoardId,
                request.TargetUserId,
                request.NewRole,
                cancellationToken);

            if (rowsAffected == 0)
            {
                throw new NotFoundException(
                    $"BoardMember record for user {request.TargetUserId} " +
                    $"was not found during update.");
            }

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        await _boardNotificationService.SendMemberRoleUpdatedAsync(
            request.BoardId, 
            request.TargetUserId, 
            request.NewRole);

        return Unit.Value;
    }
}
