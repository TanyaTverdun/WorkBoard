using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

/// <summary>
/// Implementation of the <see cref="IUserRepository"/>
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    /// <inheritdoc />
    public async Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _connectionFactory.GetOrCreateConnection();

        string sql = $"""
            SELECT UserId, FullName, Email, AvatarUrl 
            FROM {_tableName} 
            WHERE UserId = @UserId
            """;
        var command = new CommandDefinition(
            sql,
            parameters: new 
            { 
                UserId = userId 
            },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>(command);
    }

    /// <inheritdoc />
    public async Task<bool> AddAsync(
        User user,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _connectionFactory.GetOrCreateConnection();

        string sql = $"""
            INSERT INTO {_tableName} (
                UserId, FullName, Email, AvatarUrl)
            VALUES 
                (@UserId, @FullName, @Email, @AvatarUrl)
            """;

        var command = new CommandDefinition(
            sql,
            parameters: new
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl
            },
            transaction: transaction,
            cancellationToken: cancellationToken);

        int rowsAffected = await connection.ExecuteAsync(command);
        return rowsAffected > 0;
    }
}
