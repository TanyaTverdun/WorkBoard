using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Application.Common.Models;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Models;
using WorkBoard.Persistence.Models;

namespace WorkBoard.Persistence.Repositories;

public class CardRepository : GenericRepository<Card, Guid>, ICardRepository
{
    public CardRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal CardRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<CardSummaryModel>> GetCardsByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                c.CardId AS Id,
                c.SectionId,
                c.Title,
                c.Description,
                c.DueDate,
                c.Position,
                (SELECT 
                    COUNT(*) 
                FROM 
                    Coments cm 
                WHERE 
                    cm.CardId = c.CardId) AS CommentsCount,
                (SELECT 
                    COUNT(*) 
                FROM 
                    Attachments a 
                WHERE 
                    a.CardId = c.CardId) AS AttachmentsCount,
                (SELECT 
                    COUNT(*) 
                FROM 
                    Checklist_items ci 
                JOIN 
                    Checklists ch 
                    ON ci.ChecklistId = ch.ChecklistId 
                WHERE 
                    ch.CardId = c.CardId) AS ChecklistTotalItems,
                (SELECT 
                    COUNT(*) 
                FROM 
                    Checklist_items ci 
                JOIN 
                    Checklists ch 
                    ON ci.ChecklistId = ch.ChecklistId 
                WHERE 
                    ch.CardId = c.CardId 
                    AND ci.IsDone = 1) AS ChecklistDoneItems
            FROM 
                Cards c
            JOIN 
                Sections s 
                ON c.SectionId = s.SectionId
            WHERE 
                s.BoardId = @BoardId
            ORDER BY 
                c.Position ASC;

            SELECT 
                cl.CardId, 
                l.LabelId AS Id,
                l.BoardId, 
                l.Name, 
                l.Color
            FROM 
                CardLabels cl
            JOIN 
                Labels l 
                ON cl.LabelId = l.LabelId
            WHERE 
                l.BoardId = @BoardId
            ORDER BY 
                l.CreatedAt ASC;

            SELECT 
                uc.CardId, 
                u.UserId, 
                u.FullName,
                u.Email,
                u.AvatarUrl
            FROM 
                UserCards uc
            JOIN 
                Users u 
                ON uc.UserId = u.UserId
            JOIN 
                Cards c 
                ON uc.CardId = c.CardId
            JOIN 
                Sections s 
                ON c.SectionId = s.SectionId
            WHERE 
                s.BoardId = @BoardId;
        ";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                BoardId = boardId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);

        var cards = (await multi.ReadAsync<CardSummaryModel>()).ToList();
        var labels = await multi.ReadAsync<CardLabelModel>();
        var assignees = await multi.ReadAsync<CardAssigneeModel>();

        var labelsDict = labels
            .GroupBy(l => l.CardId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var assigneesDict = assignees
            .GroupBy(a => a.CardId)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var card in cards)
        {
            if (labelsDict.TryGetValue(card.Id, out var cardLabels))
            {
                card.Labels = cardLabels.Select(l => new LabelModel
                {
                    Id = l.Id,
                    BoardId = l.BoardId,
                    Name = l.Name,
                    Color = l.Color
                }).ToList();
            }

            if (assigneesDict.TryGetValue(card.Id, out var cardAssignees))
            {
                card.Assignees = cardAssignees.Select(a => new AssigneeModel
                {
                    UserId = a.UserId,
                    FullName = a.FullName,
                    Email = a.Email,
                    AvatarUrl = a.AvatarUrl
                }).ToList();
            }
        }

        return cards.AsReadOnly();
    }

    public async Task<CardFullDataModel?> GetCardFullDataAsync(
        Guid cardId,
        CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT 
                CardId AS Id, 
                SectionId, 
                Title, 
                Description, 
                DueDate, 
                Position 
            FROM 
                Cards 
            WHERE 
                CardId = @CardId;

            SELECT 
                u.UserId AS Id, 
                u.FullName, 
                u.Email, 
                u.AvatarUrl 
            FROM 
                UserCards uc 
            INNER JOIN 
                Users u 
                ON uc.UserId = u.UserId 
            WHERE 
                uc.CardId = @CardId;

            SELECT 
                l.LabelId AS Id, 
                l.BoardId, 
                l.Name, 
                l.Color 
            FROM 
                Labels l 
            INNER JOIN 
                CardLabels cl 
                ON l.LabelId = cl.LabelId 
            WHERE 
                cl.CardId = @CardId
            ORDER BY 
                l.CreatedAt ASC;

            SELECT 
                ChecklistId AS Id, 
                CardId, 
                Name 
            FROM 
                Checklists 
            WHERE 
                CardId = @CardId;

            SELECT 
                ci.ChecklistItemId AS Id, 
                ci.ChecklistId, 
                ci.Title, 
                ci.IsDone 
            FROM 
                Checklist_items ci 
            INNER JOIN 
                Checklists c 
                ON ci.ChecklistId = c.ChecklistId 
            WHERE 
                c.CardId = @CardId 
            ORDER BY 
                ci.CreatedAt ASC;

            SELECT 
                AttachmentId AS Id, 
                FileUrl, 
                FileName, 
                FileSizeBytes, 
                CreatedAt, 
                CreatedBy 
            FROM 
                Attachments 
            WHERE 
                CardId = @CardId 
            ORDER BY 
                CreatedAt ASC;

            SELECT 
                c.ComentId AS Id, 
                c.UserId, 
                c.Text, 
                c.CreatedAt, 
                u.FullName AS UserFullName 
            FROM 
                Coments c 
            INNER JOIN 
                Users u 
                ON c.UserId = u.UserId 
            WHERE 
                c.CardId = @CardId 
            ORDER BY 
                c.CreatedAt ASC;

            SELECT 
               al.ActivityLogId AS Id,
               al.CardId,
               al.UserId,
               al.Text,
               al.CreatedAt,
               u.FullName 
            FROM 
                ActivityLogs al
            INNER JOIN 
                Users u 
                ON al.UserId = u.UserId
            WHERE 
                CardId = @CardId 
            ORDER BY 
                CreatedAt DESC;
        ";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                CardId = cardId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        using var multi = await _connection.QueryMultipleAsync(command);

        var card = await multi.ReadSingleOrDefaultAsync<Card>();
        if (card == null)
        {
            return null;
        }

        var result = new CardFullDataModel 
        { 
            Card = card 
        };

        result.Assignees = (await multi.ReadAsync<User>()).ToList();
        result.Labels = (await multi.ReadAsync<Label>()).ToList();

        var checklist = await multi.ReadSingleOrDefaultAsync<Checklist>();
        var checklistItems = (await multi.ReadAsync<ChecklistItem>()).ToList();

        if (checklist != null)
        {
            checklist.Items = checklistItems;
            result.Checklist = checklist;
        }

        result.Attachments = (await multi.ReadAsync<Attachment>()).ToList();
        result.Comments = (await multi.ReadAsync<Comment>()).ToList();
        result.ActivityLogs = (await multi.ReadAsync<ActivityLog>()).ToList();

        return result;
    }

    public async Task UpdateCardPositionAsync(
        Guid cardId,
        Guid sectionId,
        double position,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE 
                Cards 
            SET 
                SectionId = @SectionId, 
                Position = @Position,
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE 
                CardId = @CardId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId,
                SectionId = sectionId,
                Position = position
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }
}
