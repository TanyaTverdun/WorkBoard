using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using System.Text;
using WorkBoard.Archivation.Services.Abstractions.Interfaces;
using WorkBoard.Archivation.Services.Options;

namespace WorkBoard.Archivation.Services.Services;

public class BlobArchivationService : IBlobArchivationService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageOptions _blobOptions;

    public BlobArchivationService(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> blobOptions)
    {
        _blobServiceClient = blobServiceClient;
        _blobOptions = blobOptions.Value;
    }

    public async Task UploadArchiveAsync(
        string fileName, 
        string jsonContent)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(
            _blobOptions.ContainerName);

        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));

        await blobClient.UploadAsync(
            stream, 
            overwrite: true);
    }
}
