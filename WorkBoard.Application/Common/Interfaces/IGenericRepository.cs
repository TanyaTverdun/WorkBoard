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
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains the entity if found; 
    /// otherwise, null
    /// </returns>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all entities of type <typeparamref name="T"> asynchronously
    /// </summary>
    /// <returns>
    /// A Task that represents the asynchronous operation.
    /// The task result contains a read-only list of all entities
    /// </returns>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Add new entity of type <typeparanref name="T"> asynchronously
    /// </summary>
    /// <param name="entity">
    /// The entity to be added
    /// </param>
    /// <returns>
    /// A Task that represents the asynchronous operation
    /// </returns>
    Task AddAsync(T entity);

    /// <summary>
    /// Undate an existing entity in database
    /// </summary>
    /// <param name="entity">
    /// The entity to be updated
    /// </param>
    /// <returns>
    /// A Task that represent the asynchronous operation
    /// </returns>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Delete entity by id in database
    /// </summary>
    /// <param name="id">
    /// The unique indetifaer of the entity
    /// </param>
    /// <returns>
    /// A Task that represent asynchronous operation
    /// </returns>
    Task DeleteAsync(Guid id);
}
