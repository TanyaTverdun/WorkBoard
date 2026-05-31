using System.Data;

namespace WorkBoard.Application.Common.Interfaces;

/// <summary>
/// Maintance database transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Begins a new database transaction
    /// </summary>
    public void BeginTransaction();

    /// <summary>
    /// Commit all changes made in current transaction to database
    /// </summary>
    /// <param name="cancellationToken">
    /// To cancel operation
    /// </param>
    /// <returns>
    /// A Task that represents the asynchronous operation
    /// </returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current active database transaction, if one has been started
    /// </summary>
    IDbTransaction? CurrentTransaction { get; }
}
