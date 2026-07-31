using WorkBoard.Archivation.Domain.DTOs;
using WorkBoard.Archivation.Domain.Enums;

namespace WorkBoard.Archivation.DataAccess.Abstractions.Interfaces;

public interface IBoardArchiveRepository
{
    Task<BoardArchiveDto?> GetBoardArchiveDataAsync(
        Guid boardId, 
        CancellationToken cancellationToken = default);

    Task SetArchiveStatusAsync(
        Guid boardId,
        BoardArchiveStatus archiveStatus,
        CancellationToken cancellationToken = default);
}
