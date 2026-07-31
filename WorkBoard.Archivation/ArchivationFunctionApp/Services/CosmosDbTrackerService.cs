using ArchivationFunctionApp.Interfaces;
using ArchivationFunctionApp.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace ArchivationFunctionApp.Services;

public class CosmosDbTrackerService : IArchivationTrackerService
{
    private readonly CosmosClient _cosmosClient;
    private readonly CosmosDbOptions _options;

    public CosmosDbTrackerService(
        CosmosClient cosmosClient,
        IOptions<CosmosDbOptions> options)
    {
        _cosmosClient = cosmosClient;
        _options = options.Value;
    }

    public async Task TrackStatusAsync(
        Guid boardId, 
        string status, 
        string? details = null)
    {
        var databaseResponse = await _cosmosClient
            .CreateDatabaseIfNotExistsAsync(_options.DatabaseName);

        var containerResponse = await databaseResponse.Database.CreateContainerIfNotExistsAsync(
            id: _options.ContainerName,
            partitionKeyPath: $"/{nameof(boardId)}");

        var container = containerResponse.Container;

        var logEntry = new
        {
            id = Guid.NewGuid().ToString(),
            boardId = boardId.ToString(),
            status = status,
            details = details,
            timestamp = DateTime.UtcNow
        };

        await container.CreateItemAsync(
            item: logEntry,
            partitionKey: new PartitionKey(logEntry.boardId));
    }
}
