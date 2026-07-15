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
}
