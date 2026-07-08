using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class CommentRepository 
    : GenericRepository<Comment, Guid>, ICommentRepository
{
    public CommentRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal CommentRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<Comment>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                c.ComentId AS Id,
                c.CardId,
                c.UserId,
                c.Text,
                c.CreatedAt,
                u.FullName AS UserFullName
            FROM 
                Coments c
            INNER JOIN 
                Users u ON c.UserId = u.UserId
            WHERE 
                c.CardId = @CardId
            ORDER BY 
                c.CreatedAt ASC;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var comments = await _connection.QueryAsync<Comment>(command);

        return comments.ToList().AsReadOnly();
    }
}
