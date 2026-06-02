using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Common;

namespace WorkBoard.Persistence.Repositories;

public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
{
    protected readonly IDbConnection _connection;
    protected readonly IDbTransaction? _transaction;

    public GenericRepository(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.Create();
        _transaction = null;
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
    }

    protected GenericRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLServer);
    }

    public async Task<TEntity?> GetByIdAsync(
        TId id, 
        CancellationToken cancellationToken = default)
    {
        return await _connection.GetAsync<TEntity?>(
            id, 
            transaction: _transaction);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var result = await _connection.GetListAsync<TEntity>(
            null, 
            transaction: _transaction);

        return result.ToList().AsReadOnly();
    }

    public Task<TId> CreateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        return _connection.InsertAsync<TId, TEntity>(
            entity, 
            transaction: _transaction);
    }

    public Task<int> UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        return _connection.UpdateAsync(
            entity, 
            transaction: _transaction);
    }

    public Task<int> DeleteAsync(
        TId id, 
        CancellationToken cancellationToken = default)
    {
        return _connection.DeleteAsync<TEntity>(
            id, 
            transaction: _transaction);
    }
}
