using MediatR;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Features.Boards.Commands.CreateBoard;

public class CreateBoardCommandHandler 
    : IRequestHandler<CreateBoardCommand, Guid>
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserContext _userContext;

    public CreateBoardCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserContext userContext)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userContext = userContext;
    }

    public async Task<Guid> Handle(
        CreateBoardCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        using var uow = _unitOfWorkFactory.Create();

        var isWorkspaceMember = await uow.WorkspaceMemberRepository.IsMemberAsync(
            request.WorkspaceId,
            currentUserId,
            cancellationToken);

        if (!isWorkspaceMember)
        {
            throw new ForbiddenAccessException(
                "You do not have access to this workspace.");
        }

        var boardId = Guid.NewGuid();

        var board = new Board
        {
            BoardId = boardId,
            WorkspaceId = request.WorkspaceId,
            Name = request.Name,
            IsArchived = false,
            ArchiveStatus = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId
        };

        var boardMember = new BoardMember
        {
            UserId = currentUserId,
            BoardId = boardId,
            UserRole = BoardRole.Member
        };

        try
        {
            await uow.BoardRepository.CreateAsync(
                board, 
                cancellationToken);

            await uow.BoardMemberRepository.AddMemberAsync(
                boardMember, 
                cancellationToken);

            uow.Commit();
        }
        catch
        {
            uow.Rollback();
            throw;
        }

        return boardId;
    }
}
