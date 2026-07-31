using ArchivationFunctionApp.DTOs;
using ArchivationFunctionApp.Enums;

namespace ArchivationFunctionApp.Interfaces;

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
