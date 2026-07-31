using ArchivationFunctionApp.Constants;
using ArchivationFunctionApp.DTOs;
using ArchivationFunctionApp.Enums;
using ArchivationFunctionApp.Interfaces;
using ArchivationFunctionApp.Options;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ArchivationFunctionApp;

public class BoardArchivationTrigger
{
    private readonly ILogger<BoardArchivationTrigger> _logger;
    private readonly IBoardArchiveRepository _repository;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageOptions _blobOptions;

    public BoardArchivationTrigger(
        ILogger<BoardArchivationTrigger> logger,
        IBoardArchiveRepository repository,
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> blobOptions)
    {
        _logger = logger;
        _repository = repository;
        _blobServiceClient = blobServiceClient;
        _blobOptions = blobOptions.Value;
    }

    [Function(nameof(BoardArchivationTrigger))]
    public async Task Run(
        [ServiceBusTrigger(
            queueName: ServiceBusConstants.ArchivationQueue,
            Connection = ServiceBusConstants.ConnectionStringKey)]        
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing archive message ID: {id}", 
            message.MessageId);

        string bodyText = message.Body.ToString();
        Guid boardId = Guid.Empty;

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var parsedMessage = JsonSerializer.Deserialize<ArchiveQueueMessage>(
                bodyText, 
                options);

            if (parsedMessage != null && parsedMessage.BoardId != Guid.Empty)
            {
                boardId = parsedMessage.BoardId;
            }
        }
        catch (JsonException)
        {
            Guid.TryParse(bodyText.Trim('"'), out boardId);
        }

        if (boardId == Guid.Empty)
        {
            _logger.LogError(
                "Invalid BoardId in message body: {body}",
                bodyText);

            await messageActions.DeadLetterMessageAsync(
                message,
                deadLetterReason: "InvalidBoardId",
                deadLetterErrorDescription: "Body does not contain a valid BoardId Guid.",
                cancellationToken: cancellationToken);

            return;
        }

        try
        {
            var boardArchive = await _repository.GetBoardArchiveDataAsync(
                boardId,
                cancellationToken);

            if (boardArchive == null)
            {
                _logger.LogWarning(
                    "Board with ID {boardId} was not found in database.", 
                    boardId);
                await messageActions.CompleteMessageAsync(message);
                return;
            }

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonContent = JsonSerializer.Serialize(
                boardArchive, 
                jsonOptions);

            var containerClient = _blobServiceClient.GetBlobContainerClient(
                _blobOptions.ContainerName);

            await containerClient.CreateIfNotExistsAsync();

            string fileName = $"board_{boardId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            await blobClient.UploadAsync(stream, overwrite: true);

            _logger.LogInformation(
                "Successfully archived board {boardId} to blob {fileName}", 
                boardId, 
                fileName);

            await _repository.SetArchiveStatusAsync(
                boardId,
                BoardArchiveStatus.Archived,
                cancellationToken);

            await messageActions.CompleteMessageAsync(
                message,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Failed to archive board {boardId}", 
                boardId);

            throw;
        }
    }
}