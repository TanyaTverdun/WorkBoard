using MediatR;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;
using WorkspaceEntity = WorkBoard.Domain.Entities.Workspace;

namespace WorkBoard.Application.Features.Workspace.Commands.CreateWorkspace;

public class CreateWorkspaceCommandHandler : 
    IRequestHandler<CreateWorkspaceCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public CreateWorkspaceCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(
        CreateWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        var workspaceId = Guid.NewGuid();

        var workspace = new WorkspaceEntity
        {
            Id = workspaceId,
            Name = request.Name,
            SubscriptionTier = SubscriptionTier.Free,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        var workspaceMember = new WorkspaceMember
        {
            UserId = currentUserId,
            WorkspaceId = workspaceId,
            UserRole = WorkspaceRole.Owner
        };

        using var uow = _unitOfWorkFactory.Create();

        try
        {
            await uow.WorkspaceRepository.CreateAsync(
                workspace, 
                cancellationToken);

            await uow.WorkspaceMemberRepository.AddMemberAsync(
                workspaceMember, 
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return workspaceId;
    }
}
