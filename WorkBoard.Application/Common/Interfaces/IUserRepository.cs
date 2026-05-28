using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces;

/// <summary>
/// Defines specific data operations for the <see cref="User"/> entity
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Retrieves a user by their unique email address
    /// </summary>
    /// <param name="email">
    /// The email address to search for
    /// </param>
    /// <param name="cancellationToken">
    /// To cancel the operation
    /// </param>
    /// <returns>
    /// The user if found; otherwise, null
    /// </returns>
    Task<User?> GetByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user profile details (Full Name and Avatar URL)
    /// </summary>
    /// <param name="user">
    /// The user entity containing updated data
    /// </param>
    /// <param name="cancellationToken">
    /// To cancel the operation
    /// </param>
    /// <returns>
    /// True if the update was successful; otherwise, false
    /// </returns>
    Task<bool> UpdateProfileAsync(
        User user, 
        CancellationToken cancellationToken = default);
}