using WorkBoard.Domain.Common;

namespace WorkBoard.Application.Common.Interfaces;

public interface IGenericRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<TId> CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    Task<int> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);


    Task<int> DeleteAsync(
        TId id,
        CancellationToken cancellationToken = default);
}
