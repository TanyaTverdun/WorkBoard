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
}
