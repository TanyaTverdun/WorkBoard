using Dapper;
using System.Data;
using WorkBoard.Archivation.DataAccess.Abstractions.Interfaces;
using WorkBoard.Archivation.Domain.DTOs;
using WorkBoard.Archivation.Domain.Enums;

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
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        using var transaction = _connection.BeginTransaction();

        try
        {
            const string sql = @"
                DELETE FROM 
                    [Sections] 
                WHERE 
                    BoardId = @BoardId;
            
                DELETE 
                    FROM [Labels] 
                WHERE 
                    BoardId = @BoardId;
            
                DELETE FROM 
                    [BoardMembers] 
                WHERE 
                    BoardId = @BoardId;
            
                UPDATE 
                    [Boards] 
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
                transaction: transaction,
                cancellationToken: cancellationToken);

            await _connection.ExecuteAsync(command);
        
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task RestoreBoardDataAsync(
        BoardArchiveDto board,
        CancellationToken cancellationToken = default)
    {
        if (_connection.State != ConnectionState.Open)
        {
            _connection.Open();
        }

        using var transaction = _connection.BeginTransaction();

        try
        {
            const string insertMembersSql = @"
                INSERT INTO [BoardMembers] (
                    UserId, 
                    BoardId, 
                    UserRole) 
                VALUES (
                    @UserId, 
                    @BoardId, 
                    @UserRole);";

            var membersToInsert = board.Members.Select(
                m => new 
                { 
                    m.UserId, 
                    board.BoardId, 
                    m.UserRole 
                });

            await _connection.ExecuteAsync(
                insertMembersSql, 
                membersToInsert, 
                transaction);

            const string insertLabelsSql = @"
                INSERT INTO [Labels] (
                    LabelId, 
                    BoardId, 
                    Name, 
                    Color, 
                    CreatedAt, 
                    CreatedBy, 
                    UpdatedAt, 
                    UpdatedBy) 
                VALUES (
                    @LabelId, 
                    @BoardId, 
                    @Name, 
                    @Color, 
                    @CreatedAt, 
                    @CreatedBy, 
                    @UpdatedAt, 
                    @UpdatedBy);";

            await _connection.ExecuteAsync(
                    insertLabelsSql, 
                    board.Labels, 
                    transaction);

            const string insertSectionsSql = @"
                INSERT INTO [Sections] (
                    SectionId, 
                    BoardId, 
                    Name, 
                    Position, 
                    CreatedAt, 
                    CreatedBy, 
                    UpdatedAt, 
                    UpdatedBy) 
                VALUES (
                    @SectionId, 
                    @BoardId, 
                    @Name, 
                    @Position, 
                    @CreatedAt, 
                    @CreatedBy, 
                    @UpdatedAt, 
                    @UpdatedBy);";

            await _connection.ExecuteAsync(
                insertSectionsSql, 
                board.Sections, 
                transaction);

            var allCards = new List<CardArchiveDto>();
            var allCardLabels = new List<object>();
            var allCardUsers = new List<object>();
            var allActivityLogs = new List<ActivityLogArchiveDto>();
            var allAttachments = new List<AttachmentArchiveDto>();
            var allComments = new List<CommentArchiveDto>();
            var allChecklists = new List<ChecklistArchiveDto>();
            var allChecklistItems = new List<ChecklistItemArchiveDto>();

            foreach (var section in board.Sections)
            {
                foreach (var card in section.Cards)
                {
                    card.SectionId = section.SectionId;
                    allCards.Add(card);

                    allCardLabels.AddRange(
                        card.LabelIds.Select(
                            labelId => new 
                            { 
                                card.CardId, 
                                labelId 
                            }));

                    allCardUsers.AddRange(
                        card.AssignedUserIds.Select(
                            userId => new 
                            { 
                                card.CardId, 
                                userId 
                            }));

                    foreach (var log in card.ActivityLogs) 
                    { 
                        log.CardId = card.CardId; 
                        allActivityLogs.Add(log); 
                    }

                    foreach (var att in card.Attachments) 
                    { 
                        att.CardId = card.CardId; 
                        allAttachments.Add(att); 
                    }

                    foreach (var com in card.Comments) 
                    { 
                        com.CardId = card.CardId; 
                        allComments.Add(com); 
                    }

                    if (card.Checklist != null)
                    {
                        card.Checklist.CardId = card.CardId;
                        allChecklists.Add(card.Checklist);
                        foreach (var item in card.Checklist.Items)
                        {
                            item.ChecklistId = card.Checklist.ChecklistId;
                            allChecklistItems.Add(item);
                        }
                    }
                }
            }

            const string insertCardsSql = @"
                INSERT INTO [Cards] (
                    CardId, 
                    SectionId, 
                    Title, 
                    Description, 
                    DueDate, 
                    Position, 
                    CreatedAt, 
                    CreatedBy, 
                    UpdatedAt, 
                    UpdatedBy
                ) 
                VALUES (
                    @CardId, 
                    @SectionId, 
                    @Title, 
                    @Description, 
                    @DueDate, 
                    @Position, 
                    @CreatedAt, 
                    @CreatedBy, 
                    @UpdatedAt, 
                    @UpdatedBy
                );";

            await _connection.ExecuteAsync(
                insertCardsSql, 
                allCards, 
                transaction);

            const string insertCardLabelsSql = @"
                INSERT INTO [CardLabels] (
                    CardId, 
                    LabelId
                ) 
                VALUES (
                    @CardId, 
                    @LabelId
                );";

            await _connection.ExecuteAsync(
                insertCardLabelsSql, 
                allCardLabels, 
                transaction);

            const string insertUserCardsSql = @"
                INSERT INTO [UserCards] (
                    UserId, 
                    CardId
                ) 
                VALUES (
                    @UserId, 
                    @CardId
                );";

            await _connection.ExecuteAsync(
                insertUserCardsSql, 
                allCardUsers, 
                transaction);

            const string insertActivitySql = @"
                INSERT INTO [ActivityLogs] (
                    ActivityLogId, 
                    CardId, 
                    UserId, 
                    Text, 
                    CreatedAt
                ) 
                VALUES (
                    @ActivityLogId, 
                    @CardId, 
                    @UserId, 
                    @Text, 
                    @CreatedAt
                );";

            await _connection.ExecuteAsync(
                insertActivitySql, 
                allActivityLogs, 
                transaction);

            const string insertAttachmentsSql = @"
                INSERT INTO [Attachments] (
                    AttachmentId, 
                    CardId, 
                    FileUrl, 
                    FileName, 
                    FileSizeBytes, 
                    CreatedAt, 
                    CreatedBy
                ) 
                VALUES (
                    @AttachmentId, 
                    @CardId, 
                    @FileUrl, 
                    @FileName, 
                    @FileSizeBytes, 
                    @CreatedAt, 
                    @CreatedBy
                );";

            await _connection.ExecuteAsync(
                insertAttachmentsSql, 
                allAttachments, 
                transaction);

            const string insertCommentsSql = @"
                INSERT INTO [Coments] (
                    ComentId, 
                    CardId, 
                    UserId, 
                    Text, 
                    CreatedAt
                ) 
                VALUES (
                    @ComentId, 
                    @CardId, 
                    @UserId, 
                    @Text, 
                    @CreatedAt
                );";

            await _connection.ExecuteAsync(
                insertCommentsSql, 
                allComments, 
                transaction);

            const string insertChecklistSql = @"
                INSERT INTO [Checklists] (
                    ChecklistId, 
                    CardId, 
                    Name, 
                    CreatedAt, 
                    CreatedBy, 
                    UpdatedAt, 
                    UpdatedBy
                ) 
                VALUES (
                    @ChecklistId, 
                    @CardId, 
                    @Name, 
                    @CreatedAt, 
                    @CreatedBy, 
                    @UpdatedAt, 
                    @UpdatedBy
                );";

            await _connection.ExecuteAsync(
                insertChecklistSql, 
                allChecklists, 
                transaction);

            const string insertChecklistItemSql = @"
                INSERT INTO [Checklist_items] (
                    ChecklistItemId, 
                    ChecklistId, 
                    Title, 
                    IsDone, 
                    CreatedAt, 
                    CreatedBy, 
                    UpdatedAt, 
                    UpdatedBy
                ) 
                VALUES (
                    @ChecklistItemId, 
                    @ChecklistId, 
                    @Title, 
                    @IsDone, 
                    @CreatedAt, 
                    @CreatedBy, 
                    @UpdatedAt, 
                    @UpdatedBy
                );";

            await _connection.ExecuteAsync(
                insertChecklistItemSql, 
                allChecklistItems, 
                transaction);

            const string updateBoardSql = @"
                UPDATE [Boards] 
                SET 
                    IsArchived = 0, 
                    ArchiveStatus = 0
                WHERE 
                    BoardId = @BoardId;";

            await _connection.ExecuteAsync(
                updateBoardSql,
                new { 
                    board.BoardId 
                },
                transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
