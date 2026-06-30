using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.RemoveBoardMember;

public class RemoveBoardMemberCommandHandler
: IRequestHandler<RemoveBoardMemberCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;

    public RemoveBoardMemberCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService boardNotificationService  )
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _boardNotificationService = boardNotificationService;
    }

    public async Task<Unit> Handle(
        RemoveBoardMemberCommand request,
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

        var workspaceMembership = await uow.WorkspaceMemberRepository
            .GetMembershipAsync(
                currentUserId,
                board.WorkspaceId,
                cancellationToken);

        if (workspaceMembership == null ||
           workspaceMembership.UserRole == WorkspaceRole.Observer || 
           currentUserId == request.TargetUserId)
        {
            throw new ForbiddenAccessException(
                "Observers cannot remove other members from the board.");
        }

        try
        {
            var rowsAffected = await uow.BoardMemberRepository.RemoveMemberAsync(
                request.BoardId,
                request.TargetUserId,
                cancellationToken);

            if (rowsAffected == 0)
            {
                throw new NotFoundException(
                    $"User {request.TargetUserId} is not a member " +
                    $"of board {request.BoardId}.");
            }

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        await _boardNotificationService.SendMemberRemovedAsync(
            request.BoardId, 
            request.TargetUserId);

        return Unit.Value;
    }
}
