using MediatR;
using WorkBoard.Application.Common.Dtos.BoardMembers;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

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

    Task<int> UpdateRoleAsync(
        Guid boardId,
        Guid userId,
        BoardRole newRole,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Guid boardId,
        Guid userId,
        BoardRole role,
        CancellationToken cancellationToken = default);
}
