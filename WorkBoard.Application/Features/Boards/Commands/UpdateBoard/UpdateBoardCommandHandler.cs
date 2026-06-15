using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.UpdateBoard;

public class UpdateBoardCommandHandler : IRequestHandler<UpdateBoardCommand>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public UpdateBoardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task Handle(
        UpdateBoardCommand request, 
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
                "You do not have permission to update boards in this workspace.");
        }

        board.Name = request.Name;
        board.UpdatedAt = DateTime.UtcNow;
        board.UpdatedBy = currentUserId;

        try
        {
            await uow.BoardRepository.UpdateAsync(
                board, 
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
