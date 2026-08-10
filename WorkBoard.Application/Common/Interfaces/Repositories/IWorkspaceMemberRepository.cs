using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IWorkspaceMemberRepository : 
    IGenericRepository<WorkspaceMember, (Guid, Guid)>
{
    Task<IEnumerable<Guid>> AddMemberAsync(
        WorkspaceMember member, 
        CancellationToken cancellationToken = default);

    Task<bool> IsMemberAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<WorkspaceMember?> GetMembershipAsync(
        Guid userId,
        Guid workspaceId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Guid>> UpdateRoleAsync(
        Guid workspaceId, 
        Guid userId, 
        WorkspaceRole newRole, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Guid>> RemoveMemberAsync(
        Guid workspaceId, 
        Guid userId, 
        CancellationToken cancellationToken = default);
}
