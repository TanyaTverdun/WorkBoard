using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Workspace.Commands.AddWorkspaceMember;

public class AddWorkspaceMemberCommandHandler
    : IRequestHandler<AddWorkspaceMemberCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBoardNotificationService _boardNotificationService;
    private readonly IWorkspaceNotificationService _workspaceNotificationService;

    public AddWorkspaceMemberCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBoardNotificationService boardNotificationService,
        IWorkspaceNotificationService workspaceNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _boardNotificationService = boardNotificationService;
        _workspaceNotificationService = workspaceNotificationService;
    }

    public async Task<Unit> Handle(
        AddWorkspaceMemberCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var currentUserMembership = await uow.WorkspaceMemberRepository.GetMembershipAsync(
            currentUserId,
            request.WorkspaceId,
            cancellationToken);

        if (currentUserMembership == null || 
            currentUserMembership.UserRole == WorkspaceRole.Observer)
        {
            throw new ForbiddenAccessException(
                "Observers cannot add new members to the workspace.");
        }

        var isTargetUserAlreadyMember = await uow.WorkspaceMemberRepository.IsMemberAsync(
            request.WorkspaceId,
            request.TargetUserId,
            cancellationToken);

        if (isTargetUserAlreadyMember)
        {
            throw new ArgumentException(
                $"User {request.TargetUserId} is already a member of this workspace.");
        }

        var newMember = _mapper.Map<WorkspaceMember>(request);

        IEnumerable<Guid> affectedBoardIds;

        try
        {
            affectedBoardIds = await uow.WorkspaceMemberRepository.AddMemberAsync(
                newMember, 
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        await _workspaceNotificationService.SendMemberAddedAsync(
            request.WorkspaceId,
            request.TargetUserId,
            request.Role,
            cancellationToken);

        if (affectedBoardIds.Any())
        {
            await _boardNotificationService.SendMemberAddedToMultipleBoardsAsync(
                affectedBoardIds,
                request.TargetUserId,
                (BoardRole)request.Role,
                cancellationToken);
        }

        return Unit.Value;
    }
}
