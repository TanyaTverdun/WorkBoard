using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Persistence.Repositories;

/// <summary>
/// Implement basic read and delete repository operations
/// </summary>
/// <typeparam name="T">
/// The type of the entity, which must be a class
/// </typeparam>
public class GenericRepository<T> : IGenericRepository<T> 
    where T : class
{
    /// <summary>
    /// The database connection factory used to open connections
    /// </summary>
    protected readonly IDbConnectionFactory _connectionFactory;

    /// <summary>
    /// The name of the database table
    /// </summary>
    protected readonly string _tableName;

    /// <summary>
    /// nitializes a new instance of the <see cref="GenericRepository{T}"/> class
    /// </summary>
    /// <param name="connectionFactory"></param>
    public GenericRepository (IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _tableName = $"{typeof(T).Name}s";
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
