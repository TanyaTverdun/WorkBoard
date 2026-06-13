using MediatR;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Workspace.Commands.UpdateWorkspace;

public class UpdateWorkspaceCommandHandler 
    : IRequestHandler<UpdateWorkspaceCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public UpdateWorkspaceCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(
        UpdateWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var workspace = await uow.WorkspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            cancellationToken);

        if (workspace == null)
        {
            throw new NotFoundException(
                $"Workspace with ID {request.WorkspaceId} was not found.");
        }

        var membership = await uow.WorkspaceMemberRepository.GetMembershipAsync(
            currentUserId,
            request.WorkspaceId,
            cancellationToken);

        if (membership == null || membership.UserRole != WorkspaceRole.Owner)
        {
            throw new ForbiddenAccessException(
                "Only the workspace owner can edit it.");
        }

        workspace.Name = request.Name;
        workspace.UpdatedAt = DateTime.UtcNow;
        workspace.UpdatedBy = currentUserId;

        try
        {
            await uow.WorkspaceRepository.UpdateAsync(workspace, cancellationToken);
            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return Unit.Value;
    }
}
