using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IWorkspaceMemberRepository : 
    IGenericRepository<WorkspaceMember, (Guid, Guid)>
{
    Task AddMemberAsync(
        WorkspaceMember member, 
        CancellationToken cancellationToken = default);

    Task<bool> IsMemberAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
