using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.AddBoardMember;

public class AddBoardMemberCommandHandler
    : IRequestHandler<AddBoardMemberCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IAppNotificationService _appNotificationService;

    public AddBoardMemberCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IAppNotificationService appNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _appNotificationService = appNotificationService;
    }

    public async Task<Unit> Handle(
        AddBoardMemberCommand request,
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

        var workspaceMembership = await uow.WorkspaceMemberRepository.GetMembershipAsync(
            currentUserId,
            board.WorkspaceId,
            cancellationToken);

        if (workspaceMembership == null ||
            workspaceMembership.UserRole == WorkspaceRole.Observer)
        {
            throw new ForbiddenAccessException(
                "Observers in the workspace cannot add members to boards.");
        }

        var isAlreadyMember = await uow.BoardMemberRepository.IsMemberAsync(
            request.BoardId,
            request.TargetUserId,
            cancellationToken);

        if (isAlreadyMember)
        {
            throw new InvalidOperationException(
                $"User {request.TargetUserId} is already a member of this board.");
        }

        bool wasAddedToWorkspace = false;

        try
        {
            var isTargetInWorkspace = await uow.WorkspaceMemberRepository.IsMemberAsync(
                board.WorkspaceId,
                request.TargetUserId,
                cancellationToken);

            if (!isTargetInWorkspace)
            {
                var newWorkspaceMember = new WorkspaceMember
                {
                    UserId = request.TargetUserId,
                    WorkspaceId = board.WorkspaceId,
                    UserRole = WorkspaceRole.Observer
                };

                await uow.WorkspaceMemberRepository.AddMemberOnlyToWorkspaceAsync(
                    newWorkspaceMember,
                    cancellationToken);

                wasAddedToWorkspace = true;
            }

            await uow.BoardMemberRepository.AddMemberAsync(
                request.BoardId,
                request.TargetUserId,
                request.Role,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        if (wasAddedToWorkspace)
        {
            await _appNotificationService.NotifyUserWorkspacesChangedAsync(
                request.TargetUserId,
                cancellationToken);
        }

        await _appNotificationService.SendSidebarBoardStatusChangedAsync(
            cancellationToken);

        return Unit.Value;
    }
}
