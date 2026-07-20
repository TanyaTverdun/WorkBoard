using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class ChecklistRepository
: GenericRepository<Checklist, Guid>, IChecklistRepository
{
    public ChecklistRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal ChecklistRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }
}
