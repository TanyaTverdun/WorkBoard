using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Workspace.Commands.DeleteWorkspace;

public class DeleteWorkspaceCommandHandler 
    : IRequestHandler<DeleteWorkspaceCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public DeleteWorkspaceCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(
        DeleteWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var workspaceExists = await uow.WorkspaceRepository.GetByIdAsync(
            request.WorkspaceId, 
            cancellationToken);

        if (workspaceExists == null)
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
                "Only the workspace owner can delete it.");
        }

        try
        {
            await uow.WorkspaceRepository.DeleteAsync(
                request.WorkspaceId,
                cancellationToken);

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
