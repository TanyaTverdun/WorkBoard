using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class WorkspaceRepository : 
    GenericRepository<Workspace, Guid>, IWorkspaceRepository
{
    public WorkspaceRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal WorkspaceRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }
}
