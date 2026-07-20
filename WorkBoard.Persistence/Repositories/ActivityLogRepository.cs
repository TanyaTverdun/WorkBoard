using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class ActivityLogRepository
    : GenericRepository<ActivityLog, Guid>, IActivityLogRepository
{
    public ActivityLogRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal ActivityLogRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }
}
