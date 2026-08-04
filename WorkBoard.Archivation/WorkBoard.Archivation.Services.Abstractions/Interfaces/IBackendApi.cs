using Refit;

namespace WorkBoard.Archivation.Services.Abstractions.Interfaces;

public interface IBackendApi
{
    [Post("/api/internal/{boardId}/archivation-completed")]
    Task NotifyArchivationCompletedAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Post("/api/internal/{boardId}/restore-completed")]
    Task NotifyRestoreCompletedAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);
}
