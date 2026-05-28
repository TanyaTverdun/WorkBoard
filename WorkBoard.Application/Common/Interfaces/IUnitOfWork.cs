namespace WorkBoard.Application.Common.Interfaces;

/// <summary>
/// Maintance database tranzactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Commit all changes made in current tranzaction to database
    /// </summary>
    /// <param name="cancellationToken">
    /// To cancel operation
    /// </param>
    /// <returns>
    /// A Task that represents the asynchronous operation
    /// </returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
