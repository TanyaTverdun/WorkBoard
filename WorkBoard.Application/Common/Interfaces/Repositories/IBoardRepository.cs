using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IBoardRepository : IGenericRepository<Board, Guid>
{
    Task<IReadOnlyList<BoardDto>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task UpdateArchiveStatusAsync(
        Guid boardId,
        bool isArchived,
        BoardArchiveStatus archiveStatus,
        Guid updatedBy,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Guid>> GetBoardIdsByArchiveStatusAsync(
        BoardArchiveStatus archiveStatus,
        CancellationToken cancellationToken = default);

    Task SetArchiveStatusAsync(
        Guid boardId,
        BoardArchiveStatus archiveStatus,
        CancellationToken cancellationToken = default);
}
