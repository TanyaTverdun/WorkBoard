using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IBoardRepository : IGenericRepository<Board, Guid>
{
    Task<IReadOnlyList<BoardDto>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
