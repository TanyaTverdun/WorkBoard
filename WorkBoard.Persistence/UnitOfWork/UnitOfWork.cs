using System.Data;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _sqlConnection;
    private readonly IDbTransaction _transaction;

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _sqlConnection = connectionFactory.GetOrCreateConnection();

        if (_sqlConnection.State == ConnectionState.Closed)
        {
            _sqlConnection.Open();
        }

        _transaction = _sqlConnection.BeginTransaction();
    }

    public void Commit()
    {
        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
    }

    public void Rollback()
    {
        _transaction.Rollback();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _sqlConnection?.Dispose();
    }
}
