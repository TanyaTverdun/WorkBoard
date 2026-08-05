using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.RestoreBoard;

public class RestoreBoardCommandHandler : IRequestHandler<RestoreBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public RestoreBoardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task Handle(
        RestoreBoardCommand request,
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
                "You do not have permission to restore boards in this workspace.");
        }

        if (!board.IsArchived ||
            board.ArchiveStatus != BoardArchiveStatus.Archived)
        {
            throw new InvalidOperationException(
                "Board is not archived or its restoration is already in progress.");
        }

        try
        {
            await uow.BoardRepository.UpdateArchiveStatusAsync(
                request.BoardId,
                true,
                BoardArchiveStatus.RestorePending,
                currentUserId,
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
