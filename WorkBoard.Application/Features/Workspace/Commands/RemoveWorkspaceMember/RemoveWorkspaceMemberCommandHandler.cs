using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Workspace.Commands.RemoveWorkspaceMember;

public class RemoveWorkspaceMemberCommandHandler
    : IRequestHandler<RemoveWorkspaceMemberCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IWorkspaceNotificationService _workspaceNotificationService;
    private readonly IBoardNotificationService _boardNotificationService;

    public RemoveWorkspaceMemberCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IWorkspaceNotificationService workspaceNotificationService,
        IBoardNotificationService boardNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _workspaceNotificationService = workspaceNotificationService;
        _boardNotificationService = boardNotificationService;
    }

    public async Task<Unit> Handle(
        RemoveWorkspaceMemberCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        if (currentUserId == request.TargetUserId)
        {
            throw new ForbiddenAccessException(
                "You cannot remove yourself");
        }

        using var uow = _unitOfWorkFactory.Create();

        var currentUserMembership = await uow.WorkspaceMemberRepository.GetMembershipAsync(
            currentUserId, 
            request.WorkspaceId, 
            cancellationToken);

        if (currentUserMembership == null || 
            currentUserMembership.UserRole == WorkspaceRole.Observer)
        {
            throw new ForbiddenAccessException(
                "Observers cannot remove members from the workspace.");
        }

        var targetUserMembership = await uow.WorkspaceMemberRepository.GetMembershipAsync(
            request.TargetUserId, 
            request.WorkspaceId, 
            cancellationToken)
                ?? throw new NotFoundException(
                    $"User {request.TargetUserId} is not a member of this workspace.");

        if (targetUserMembership.UserRole == WorkspaceRole.Owner)
        {
            throw new ForbiddenAccessException(
                "Cannot remove the Workspace Owner.");
        }

        IEnumerable<Guid> affectedBoardIds;

        try
        {
            affectedBoardIds = await uow.WorkspaceMemberRepository.RemoveMemberAsync(
                request.WorkspaceId,
                request.TargetUserId,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        await _workspaceNotificationService.SendMemberRemovedAsync(
            request.WorkspaceId,
            request.TargetUserId,
            cancellationToken);

        if (affectedBoardIds.Any())
        {
            await _boardNotificationService.SendMemberRemovedFromMultipleBoardsAsync(
                affectedBoardIds,
                request.TargetUserId,
                cancellationToken);
        }

        return Unit.Value;
    }
}
