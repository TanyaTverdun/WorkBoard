using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ArchivationFunctionApp;

public class BoardArchivationTrigger
{
    private readonly ILogger<BoardArchivationTrigger> _logger;

    public BoardArchivationTrigger(ILogger<BoardArchivationTrigger> logger)
    {
        _logger = logger;
    }

    [Function(nameof(BoardArchivationTrigger))]
    public async Task Run(
        [ServiceBusTrigger("myqueue", Connection = "")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}