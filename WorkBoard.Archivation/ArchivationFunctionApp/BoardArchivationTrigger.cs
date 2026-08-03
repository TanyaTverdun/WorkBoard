using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Text.Json;
using WorkBoard.Archivation.DataAccess.Abstractions.Interfaces;
using WorkBoard.Archivation.Domain.Constants;
using WorkBoard.Archivation.Domain.DTOs;
using WorkBoard.Archivation.Domain.Enums;
using WorkBoard.Archivation.Services.Abstractions.Interfaces;

namespace ArchivationFunctionApp;

public class BoardArchivationTrigger
{
    private readonly ILogger<BoardArchivationTrigger> _logger;
    private readonly IBoardArchiveRepository _repository;
    private readonly IBlobArchivationService _blobService;
    private readonly IArchivationTrackerService _tracker;

    public BoardArchivationTrigger(
        ILogger<BoardArchivationTrigger> logger,
        IBoardArchiveRepository repository,
        IBlobArchivationService blobService,
        IArchivationTrackerService tracker)
    {
        _logger = logger;
        _repository = repository;
        _blobService = blobService;
        _tracker = tracker;
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

            await _tracker.TrackStatusAsync(
                boardId,
                "Started",
                "Archivation process initiated.");
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

                await _tracker.TrackStatusAsync(
                    boardId, 
                    "Warning", 
                    "Board data not found in DB.");

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

            string fileName = $"board_{boardId}.json";

            await _blobService.UploadArchiveAsync(fileName, jsonContent);

            await _tracker.TrackStatusAsync(
                boardId, 
                "BlobUploaded", 
                $"File {fileName} created in Blob Storage.");

            _logger.LogInformation(
                "Successfully archived board {boardId} to blob {fileName}", 
                boardId, 
                fileName);

            await _repository.SetArchiveStatusAsync(
                boardId,
                BoardArchiveStatus.Archived,
                cancellationToken);

            await _tracker.TrackStatusAsync(
                boardId, 
                "Completed", 
                "Database status updated to Archived.");

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

            await _tracker.TrackStatusAsync(
                boardId, 
                "Failed", 
                $"Error: {ex.Message}");

            throw;
        }
    }
}