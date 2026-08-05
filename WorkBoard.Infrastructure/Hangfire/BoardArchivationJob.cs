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
            .GetBoardIdsByStatusAsync(
                BoardArchiveStatus.Pending);

        if (!pendingBoardIds.Any())
        {
            return;
        }

        await using var sender = _serviceBusClient.CreateSender(
            ServiceBusQueueNames.ArchivationQueue);

        foreach (var boardId in pendingBoardIds)
        {
            await ProcessBoardStatusAsync(boardId, sender);
        }
    }

    public async Task ProcessRestorePendingBoardsAsync()
    {
        var restorePendingBoardIds = await _boardRepository
            .GetBoardIdsByStatusAsync(
                BoardArchiveStatus.RestorePending);

        if (!restorePendingBoardIds.Any())
        {
            return;
        }

        await using var sender = _serviceBusClient.CreateSender(
            ServiceBusQueueNames.RestoreQueue);

        foreach (var boardId in restorePendingBoardIds)
        {
            await ProcessBoardStatusAsync(boardId, sender);
        }
    }

    private async Task ProcessBoardStatusAsync(
        Guid boardId, 
        ServiceBusSender sender)
    {
        using var uow = _unitOfWorkFactory.Create();
        try
        {
            await uow.BoardRepository.SetArchiveStatusAsync(
                boardId,
                BoardArchiveStatus.Queued);

            var messageContent = JsonSerializer.Serialize(
                new 
                { 
                    BoardId = boardId 
                });

            var message = new ServiceBusMessage(messageContent);

            await sender.SendMessageAsync(message);

            uow.Commit();
        }
        catch (Exception)
        {
            uow.Rollback();
        }
    }
}
