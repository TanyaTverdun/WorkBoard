using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class AttachmentRepository 
    : GenericRepository<Attachment, Guid>, IAttachmentRepository
{
    public AttachmentRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal AttachmentRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<Attachment>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                AttachmentId AS Id,
                CardId,
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
                CreatedAt ASC;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var attachments = await _connection.QueryAsync<Attachment>(command);

        return attachments.ToList().AsReadOnly();
    }
}
