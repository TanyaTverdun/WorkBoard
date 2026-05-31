using System.Data;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <inheritdoc />
    public IDbTransaction? CurrentTransaction => _transaction;

    /// <inheritdoc />
    public void BeginTransaction()
    {
        if (_transaction == null)
        {
            var connection = _connectionFactory.GetOrCreateConnection();
            _transaction = connection.BeginTransaction();
        }
    }

    /// <inheritdoc />
    public async Task CommitAsync(
        CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException(
                "Cannot commit: No transaction has been started.");
        }

        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Clears transaction resources if they weren't committed.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _transaction?.Dispose();
            _transaction = null;
            _disposed = true;
        }
    }
}
