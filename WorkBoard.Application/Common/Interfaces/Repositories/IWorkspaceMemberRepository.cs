using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IWorkspaceMemberRepository
{
    Task AddMemberAsync(
        WorkspaceMember member, 
        CancellationToken cancellationToken = default);
}
