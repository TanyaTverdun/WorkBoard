using WorkBoard.Application.Common.Dtos.Workspaces;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IWorkspaceRepository : IGenericRepository<Workspace, Guid>
{
    Task<IReadOnlyList<UserWorkspaceDto>> GetByUserIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);
}
