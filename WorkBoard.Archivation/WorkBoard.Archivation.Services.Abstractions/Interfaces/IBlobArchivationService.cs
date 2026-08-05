namespace WorkBoard.Archivation.Services.Abstractions.Interfaces;

public interface IBlobArchivationService
{
    Task UploadArchiveAsync(
        string fileName, 
        string jsonContent,
        CancellationToken cancellationToken = default);

    Task<string?> DownloadArchiveAsync(
        string fileName, 
        CancellationToken cancellationToken = default);
}