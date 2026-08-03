using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WorkBoard.Archivation.DataAccess.Abstractions.Interfaces;
using WorkBoard.Archivation.Domain.Constants;
using WorkBoard.Archivation.Domain.DTOs;
using WorkBoard.Archivation.Services.Abstractions.Interfaces;

namespace ArchivationFunctionApp;

public class BoardRestoreTrigger
{
    private readonly ILogger<BoardRestoreTrigger> _logger;
    private readonly IBoardArchiveRepository _repository;
    private readonly IBlobArchivationService _blobService;
    private readonly IArchivationTrackerService _tracker;

    public BoardRestoreTrigger(
        ILogger<BoardRestoreTrigger> logger,
        IBoardArchiveRepository repository,
        IBlobArchivationService blobService,
        IArchivationTrackerService tracker)
    {
        _logger = logger;
        _repository = repository;
        _blobService = blobService;
        _tracker = tracker;
    }

    [Function(nameof(BoardRestoreTrigger))]
    public async Task Run(
        [ServiceBusTrigger(
        queueName: ServiceBusConstants.RestoreQueue,
        Connection = ServiceBusConstants.ConnectionStringKey)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing restore message ID: {id}", 
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

            await _tracker.TrackStatusAsync(
                boardId, 
                "RestoreStarted", "Restore process initiated.");
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
                deadLetterErrorDescription: "Body does not contain a valid BoardId.",
                cancellationToken: cancellationToken);

            return;
        }

        try
        {
            string fileName = $"board_{boardId}.json";

            var jsonContent = await _blobService.DownloadArchiveAsync(
                fileName, 
                cancellationToken);

            if (string.IsNullOrEmpty(jsonContent))
            {
                _logger.LogWarning(
                    "Blob file {fileName} not found for board {boardId}.", 
                    fileName, 
                    boardId);

                await _tracker.TrackStatusAsync(
                    boardId, 
                    "RestoreFailed", 
                    "Backup file not found in Blob Storage.");

                await messageActions.DeadLetterMessageAsync(
                    message,
                    deadLetterReason: "RestoreFailed",
                    deadLetterErrorDescription: "Backup file not found in Blob Storage.",
                    cancellationToken: cancellationToken);
                return;
            }

            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };

            var boardArchive = JsonSerializer.Deserialize<BoardArchiveDto>(
                jsonContent, 
                jsonOptions);

            if (boardArchive == null)
            {
                throw new InvalidOperationException(
                    $"Failed to deserialize board backup for board ID: {boardId}. " +
                    $"The resulting object is null.");
            }

            await _tracker.TrackStatusAsync(
                boardId, 
                "BlobDownloaded", 
                "Backup data successfully loaded from storage.");

            await _repository.RestoreBoardDataAsync(
                boardArchive, 
                cancellationToken);

            await _tracker.TrackStatusAsync(
                boardId, 
                "Completed", 
                "Board successfully restored and activated.");

            _logger.LogInformation(
                "Successfully restored board {boardId}", 
                boardId);

            await messageActions.CompleteMessageAsync(
                message, 
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Failed to restore board {boardId}", 
                boardId);

            await _tracker.TrackStatusAsync(
                boardId, 
                "Failed", 
                $"Error during restore: {ex.Message}");

            throw;
        }
    }
}
