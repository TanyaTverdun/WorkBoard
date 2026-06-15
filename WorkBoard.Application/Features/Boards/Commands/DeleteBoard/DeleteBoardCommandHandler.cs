using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.DeleteBoard;

public class DeleteBoardCommandHandler : IRequestHandler<DeleteBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public DeleteBoardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task Handle(
        DeleteBoardCommand request, 
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

        if (membership == null || membership.UserRole == WorkspaceRole.Observer)
        {
            throw new ForbiddenAccessException(
                "Only workspace administrators can delete boards.");
        }

        try
        {
            await uow.BoardRepository.DeleteAsync(
                board.Id, 
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
