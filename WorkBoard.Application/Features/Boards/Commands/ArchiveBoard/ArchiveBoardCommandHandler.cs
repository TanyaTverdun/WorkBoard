using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.ArchiveBoard;

public class ArchiveBoardCommandHandler : IRequestHandler<ArchiveBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public ArchiveBoardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task Handle(
    ArchiveBoardCommand request,
    CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var board = await uow.BoardRepository.GetByIdAsync(
            request.BoardId,
            cancellationToken)
                ?? throw new KeyNotFoundException(
                    $"Board with ID {request.BoardId} was not found.");

        var membership = await uow.WorkspaceMemberRepository.GetMembershipAsync(
            currentUserId,
            board.WorkspaceId,
            cancellationToken);

        if (membership == null || 
            membership.UserRole == WorkspaceRole.Observer)
        {
            throw new ForbiddenAccessException(
                "You do not have permission to archive boards in this workspace.");
        }

        if (board.IsArchived || 
            board.ArchiveStatus != BoardArchiveStatus.Active)
        {
            throw new InvalidOperationException(
                "Board is already archived or archivation is in progress.");
        }

        try
        {
            await uow.BoardRepository.UpdateArchiveStatusAsync(
                request.BoardId,
                isArchived: true,
                archiveStatus: BoardArchiveStatus.Pending,
                updatedBy: currentUserId,
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }
}