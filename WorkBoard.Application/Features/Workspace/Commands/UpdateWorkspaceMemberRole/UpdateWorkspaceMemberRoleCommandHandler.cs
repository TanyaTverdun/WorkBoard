using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Workspace.Commands.UpdateWorkspaceMemberRole;

public class UpdateWorkspaceMemberRoleCommandHandler
    : IRequestHandler<UpdateWorkspaceMemberRoleCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IBoardNotificationService _boardNotificationService;
    private readonly IWorkspaceNotificationService _workspaceNotificationService;

    public UpdateWorkspaceMemberRoleCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IBoardNotificationService boardNotificationService,
        IWorkspaceNotificationService workspaceNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _boardNotificationService = boardNotificationService;
        _workspaceNotificationService = workspaceNotificationService;
    }

    public async Task<Unit> Handle(
        UpdateWorkspaceMemberRoleCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        if (currentUserId == request.TargetUserId)
        {
            throw new ForbiddenAccessException(
                "You cannot change your own role in the workspace.");
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
                "Observers cannot change roles in the workspace.");
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
                "Cannot change the role of the Workspace Owner.");
        }

        IEnumerable<Guid> affectedBoardIds;

        try
        {
            affectedBoardIds = await uow.WorkspaceMemberRepository.UpdateRoleAsync(
                request.WorkspaceId,
                request.TargetUserId,
                request.NewRole,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        await _workspaceNotificationService.SendMemberRoleUpdatedAsync(
            request.WorkspaceId,
            request.TargetUserId,
            request.NewRole,
            cancellationToken);

        if (affectedBoardIds.Any())
        {
            await _boardNotificationService.SendMemberRoleUpdatedInMultipleBoardsAsync(
                affectedBoardIds,
                request.TargetUserId,
                (BoardRole)request.NewRole,
                cancellationToken);
        }

        return Unit.Value;
    }
}
