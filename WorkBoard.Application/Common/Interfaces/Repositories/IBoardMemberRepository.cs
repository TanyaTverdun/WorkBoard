using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IBoardMemberRepository 
    : IGenericRepository<BoardMember, (Guid, Guid)>
{
    Task AddMemberAsync(
        BoardMember member, 
        CancellationToken cancellationToken = default);

    Task<bool> IsMemberAsync(
        Guid boardId, 
        Guid userId, 
        CancellationToken cancellationToken = default);
}
