using ArchivationFunctionApp.DTOs;
using ArchivationFunctionApp.Enums;
using ArchivationFunctionApp.Interfaces;
using Dapper;
using System.Data;

namespace ArchivationFunctionApp.Repositories;

public class BoardArchiveRepository : IBoardArchiveRepository
{
    protected readonly IDbConnection _connection;

    public BoardArchiveRepository(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.Create();
    }

    public async Task<BoardArchiveDto?> GetBoardArchiveDataAsync(
        Guid boardId, 
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
                SELECT * 
                FROM [Boards]   
                WHERE BoardId = @BoardId;

                SELECT * 
                FROM [BoardMembers] 
                WHERE BoardId = @BoardId;

                SELECT * 
                FROM [Labels] 
                WHERE BoardId = @BoardId;

                SELECT * 
                FROM [Sections] 
                WHERE BoardId = @BoardId;

                SELECT c.* 
                FROM [Cards] c
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;

                SELECT cl.* 
                FROM [CardLabels] cl
                INNER JOIN [Cards] c 
                    ON cl.CardId = c.CardId
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;

                SELECT uc.* 
                FROM [UserCards] uc
                INNER JOIN [Cards] c 
                    ON uc.CardId = c.CardId
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;

                SELECT al.* 
                FROM [ActivityLogs] al
                INNER JOIN [Cards] c 
                    ON al.CardId = c.CardId
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;

                SELECT a.* 
                FROM [Attachments] a
                INNER JOIN [Cards] c 
                    ON a.CardId = c.CardId
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;

                SELECT ch.* 
                FROM [Checklists] ch
                INNER JOIN [Cards] c 
                    ON ch.CardId = c.CardId
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;

                SELECT ci.* 
                FROM [Checklist_items] ci
                INNER JOIN [Checklists] ch 
                    ON ci.ChecklistId = ch.ChecklistId
                INNER JOIN [Cards] c 
                    ON ch.CardId = c.CardId
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;

                SELECT co.* 
                FROM [Coments] co
                INNER JOIN [Cards] c 
                    ON co.CardId = c.CardId
                INNER JOIN [Sections] s 
                    ON c.SectionId = s.SectionId
                WHERE s.BoardId = @BoardId;
            ";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                BoardId = boardId 
            },
            cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);

        var board = await multi.ReadSingleOrDefaultAsync<BoardArchiveDto>();
        if (board == null)
        {
            return null;
        }

        board.Members = (await multi.ReadAsync<BoardMemberArchiveDto>())
            .ToList();
        board.Labels = (await multi.ReadAsync<LabelArchiveDto>())
            .ToList();
        board.Sections = (await multi.ReadAsync<SectionArchiveDto>())
            .ToList();

        var cards = (await multi.ReadAsync<CardArchiveDto>())
            .ToList();
        var cardLabels = (await multi.ReadAsync<CardLabelMapping>())
            .ToList();
        var cardUsers = (await multi.ReadAsync<CardUserMapping>())
            .ToList();
        var activityLogs = (await multi.ReadAsync<ActivityLogArchiveDto>())
            .ToList();
        var attachments = (await multi.ReadAsync<AttachmentArchiveDto>())
            .ToList();
        var checklists = (await multi.ReadAsync<ChecklistArchiveDto>())
            .ToList();
        var checklistItems = (await multi.ReadAsync<ChecklistItemArchiveDto>())
            .ToList();
        var comments = (await multi.ReadAsync<CommentArchiveDto>())
            .ToList();

        foreach (var checklist in checklists)
        {
            checklist.Items = checklistItems
                .Where(i => i.ChecklistId == checklist.ChecklistId)
                .ToList();
        }

        foreach (var card in cards)
        {
            card.LabelIds = cardLabels
                .Where(cl => cl.CardId == card.CardId)
                .Select(cl => cl.LabelId)
                .ToList();

            card.AssignedUserIds = cardUsers
                .Where(cu => cu.CardId == card.CardId)
                .Select(cu => cu.UserId)
                .ToList();

            card.ActivityLogs = activityLogs
                .Where(a => a.CardId == card.CardId)
                .ToList();
            card.Attachments = attachments
                .Where(a => a.CardId == card.CardId)
                .ToList();
            card.Comments = comments
                .Where(c => c.CardId == card.CardId)
                .ToList();

            card.Checklist = checklists
                .SingleOrDefault(ch => ch.CardId == card.CardId);
        }

        foreach (var section in board.Sections)
        {
            section.Cards = cards
                .Where(c => c.SectionId == section.SectionId)
                .ToList();
        }

        return board;
    }

    public async Task SetArchiveStatusAsync(
        Guid boardId,
        BoardArchiveStatus archiveStatus,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE Boards 
            SET 
                ArchiveStatus = @ArchiveStatus
            WHERE 
                BoardId = @BoardId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                ArchiveStatus = (int)archiveStatus,
                BoardId = boardId
            },
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }
}
