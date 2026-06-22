using MediatR;
using WorkBoard.Application.Common.Exceptions;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.AddBoardMember;

public class AddBoardMemberCommandHandler
    : IRequestHandler<AddBoardMemberCommand, Unit>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public AddBoardMemberCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
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

        try
        {
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

        return Unit.Value;
    }
}
