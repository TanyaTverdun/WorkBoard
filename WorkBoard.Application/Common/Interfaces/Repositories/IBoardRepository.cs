using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Application.Common.Dtos.Boards;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IBoardRepository : IGenericRepository<Board, Guid>
{
    Task<IReadOnlyList<BoardDto>> GetForUserByWorkspaceIdAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateArchiveStatusAsync(
        Guid boardId,
        bool isArchived,
        BoardArchiveStatus archiveStatus,
        Guid updatedBy,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Guid>> GetBoardIdsByStatusAsync(
        BoardArchiveStatus archiveStatus,
        CancellationToken cancellationToken = default);

    Task SetArchiveStatusAsync(
        Guid boardId,
        BoardArchiveStatus archiveStatus,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BoardArchivationDto>> GetBoardsForArchivationAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
