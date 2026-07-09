using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using WorkBoard.Application.Common.Interfaces.BlobStorage;

namespace WorkBoard.Infrastructure.BlobStorage;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var blobClient = containerClient.GetBlobClient(uniqueFileName);

        var blobHttpHeader = new BlobHttpHeaders 
        { 
            ContentType = contentType 
        };

        await blobClient.UploadAsync(
            fileStream,
            new BlobUploadOptions 
            { 
                HttpHeaders = blobHttpHeader 
            },
            cancellationToken);

        return blobClient.Uri.ToString();
    }

    public string GetReadSasUrl(
        string fileUrl, 
        string containerName, 
        int expiresInMinutes = 1440)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            containerName);

        var uri = new Uri(fileUrl);
        var fileName = Path.GetFileName(uri.LocalPath);

        var blobClient = containerClient.GetBlobClient(fileName);

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException(
                "BlobClient cannot generate SAS URI. " +
                "Ensure you are using connection strings.");
        }

        var sasUri = blobClient.GenerateSasUri(
            BlobSasPermissions.Read,
            DateTimeOffset.UtcNow.AddMinutes(expiresInMinutes));

        return sasUri.ToString();
    }

    public async Task DeleteAsync(
        string fileUrl,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            containerName);

        var uri = new Uri(fileUrl);
        var fileName = Path.GetFileName(uri.LocalPath);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.DeleteIfExistsAsync(
            cancellationToken: cancellationToken);
    }
}