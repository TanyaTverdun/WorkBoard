using AutoMapper;
using MediatR;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Helpers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
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
    private readonly IBlobStorageService _blobStorageService;
    private readonly IAppNotificationService _appNotificationService;

    public AddWorkspaceMemberCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext,
        IMapper mapper,
        IBoardNotificationService boardNotificationService,
        IWorkspaceNotificationService workspaceNotificationService,
        IBlobStorageService blobStorageService,
        IAppNotificationService appNotificationService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
        _mapper = mapper;
        _boardNotificationService = boardNotificationService;
        _workspaceNotificationService = workspaceNotificationService;
        _blobStorageService = blobStorageService;
        _appNotificationService = appNotificationService;
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

        var targetUser = await uow.UserRepository.GetByIdAsync(
            request.TargetUserId, 
            cancellationToken);

        string? avatarUrl = targetUser.AvatarUrl;
        if (!string.IsNullOrWhiteSpace(avatarUrl))
        {
            avatarUrl = _blobStorageService.GetReadSasUrl(
                avatarUrl, 
                BlobContainers.Avatars);
        }

        var payload = new WorkspaceMemberAddedDto(
            targetUser.Id,
            targetUser.FullName,
            targetUser.Email,
            request.Role,
            avatarUrl,
            targetUser.AvatarColor,
            InitialGenerator.Generate(targetUser.FullName)
        );

        await _workspaceNotificationService.SendMemberAddedAsync(
            request.WorkspaceId,
            payload,
            cancellationToken);

        await _appNotificationService.NotifyUserWorkspacesChangedAsync(
            request.TargetUserId, 
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
