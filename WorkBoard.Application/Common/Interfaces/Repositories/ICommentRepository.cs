using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ICommentRepository : IGenericRepository<Comment, Guid>
{
    Task<IReadOnlyList<Comment>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}
