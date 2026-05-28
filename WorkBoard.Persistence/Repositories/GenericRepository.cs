using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> 
    where T : class
{
    protected readonly IDbConnectionFactory _connectionFactory;
    protected readonly string _tableName;

    public GenericRepository (IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _tableName = $"{typeof(T).Name}s";
    }

    public async Task<T?> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _connectionFactory.GetOrCreateConnection();
        string sql = $"SELECT * FROM {_tableName} WHERE Id = @Id";

        var command = new CommandDefinition(
            sql,
            parameters: new { Id = id },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<T>(command);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _connectionFactory.GetOrCreateConnection();
        string sql = $"SELECT * FROM {_tableName}";

        var command = new CommandDefinition(
            sql,
            cancellationToken: cancellationToken);

        var result = await connection.QueryAsync<T>(command);
        return result.ToList().AsReadOnly();
    }

    public async Task<bool> DeleteAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _connectionFactory.GetOrCreateConnection();
        string sql = $"DELETE FROM {_tableName} WHERE Id = @Id";

        var command = new CommandDefinition(
            sql,
            new
            {
                Id = id
            },
            cancellationToken: cancellationToken);

        int rowsAffected = await connection.ExecuteAsync(command);

        return rowsAffected > 0;
    }
}
