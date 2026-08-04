using Refit;

namespace WorkBoard.Archivation.Services.Abstractions.Interfaces;

public interface IBackendApi
{
    [Post("/api/azureFunction/{boardId}/archivation-completed")]
    Task NotifyArchivationCompletedAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    [Post("/api/azureFunction/{boardId}/restore-completed")]
    Task NotifyRestoreCompletedAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);
}
