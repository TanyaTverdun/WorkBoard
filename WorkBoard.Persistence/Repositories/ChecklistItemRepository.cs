using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class ChecklistItemRepository 
    : GenericRepository<ChecklistItem, Guid>, IChecklistItemRepository
{
    public ChecklistItemRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal ChecklistItemRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }
}
