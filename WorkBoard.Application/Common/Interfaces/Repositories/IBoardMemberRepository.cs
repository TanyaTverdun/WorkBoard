using WorkBoard.Application.Common.Dtos.BoardMembers;
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

    Task<BoardMember?> GetMembershipAsync(
        Guid userId, 
        Guid boardId, 
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BoardMemberWithUserDto>> GetMembersByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);
}
