namespace WorkBoard.Application.Common.Interfaces;

/// <summary>
/// Defines basic CRUD operations for generic repositories
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IGenericRepository<T> 
    where T : class
{
    /// <summary>
    /// Get entity by id asynchronously
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the entity
    /// </param>
    /// <param name="cancellationToken">
    /// To cancel operation
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the entity if found; 
    /// otherwise, null
    /// </returns>
    Task<T?> GetByIdAsync(Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all entities of type <typeparamref name="T"> asynchronously
    /// </summary>
    /// <param name="cancellationToken">
    /// To cancel operation
    /// </param>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains a read-only list of all entities
    /// </returns>
    Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete entity by id in database
    /// </summary>
    /// <param name="id">
    /// The unique indetifaer of the entity
    /// </param>
    /// <param name="cancellationToken">
    /// To cancel operation
    /// </param>
    /// <returns>
    /// A Task that represent asynchronous operation
    /// </returns>
    Task DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
}
