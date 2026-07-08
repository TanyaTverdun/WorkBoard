namespace WorkBoard.Application.Common.Interfaces.BlobStorage;

public interface IBlobStorageService
{
    Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string fileUrl,
        string containerName,
        CancellationToken cancellationToken = default);
}
