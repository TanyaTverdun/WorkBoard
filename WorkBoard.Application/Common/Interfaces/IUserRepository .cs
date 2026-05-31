using System.Data;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces;

/// <summary>
/// Defines data operations for the <see cref="User"/> entity
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, 
    /// containing the <see cref="User"/> if found; 
    /// otherwise, <see langword="null"/>
    /// </returns>
    Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new user to the database within an optional transaction.
    /// </summary>
    /// <param name="user">
    /// The user entity to add
    /// </param>
    /// <param name="transaction">
    /// The database transaction to execute the command within
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation, 
    /// containing true if the user was successfully added; 
    /// otherwise,false/>
    /// </returns>
    Task<bool> AddAsync(
        User user,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);
}
