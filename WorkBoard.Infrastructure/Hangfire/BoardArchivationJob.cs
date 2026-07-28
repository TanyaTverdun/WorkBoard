using Azure.Messaging.ServiceBus;
using System.Text.Json;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Enums;
using WorkBoard.Infrastructure.Constants;

namespace WorkBoard.Infrastructure.Hangfire;

public class BoardArchivationJob
{
    private readonly IBoardRepository _boardRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ServiceBusClient _serviceBusClient;

    public BoardArchivationJob(
        IBoardRepository boardRepository,
        IUnitOfWorkFactory unitOfWorkFactory,
        ServiceBusClient serviceBusClient)
    {
        _boardRepository = boardRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
        _serviceBusClient = serviceBusClient;
    }

    public async Task ProcessPendingBoardsAsync()
    {
        var pendingBoardIds = await _boardRepository
            .GetBoardIdsByArchiveStatusAsync(BoardArchiveStatus.Pending);

        if (!pendingBoardIds.Any())
        {
            return;
        }

        await using var sender = _serviceBusClient.CreateSender(
            ServiceBusQueueNames.ArchivationQueue);

        foreach (var boardId in pendingBoardIds)
        {
            using var uow = _unitOfWorkFactory.Create();
            try
            {
                await uow.BoardRepository.SetArchiveStatusAsync(
                    boardId, 
                    BoardArchiveStatus.Queued);

                uow.Commit();

                var messageContent = JsonSerializer.Serialize(
                    new 
                    { 
                        BoardId = boardId 
                    });
                var message = new ServiceBusMessage(messageContent);
                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                using var failUow = _unitOfWorkFactory.Create();
                await failUow.BoardRepository.SetArchiveStatusAsync(
                    boardId, 
                    BoardArchiveStatus.Failed);

                failUow.Commit();
            }
        }
    }
}
