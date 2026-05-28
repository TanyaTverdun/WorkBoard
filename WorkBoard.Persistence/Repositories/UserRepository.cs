using System.Data;
using Dapper;
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
    public async Task<User?> GetByEmailAsync(
        string email, 
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _connectionFactory.GetOrCreateConnection();

        string sql = $"SELECT * FROM {_tableName} WHERE Email = @Email";

        var command = new CommandDefinition(
            sql,
            parameters: new 
            { 
                Email = email 
            },
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<User>(command);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateProfileAsync(
        User user, 
        CancellationToken cancellationToken = default)
    {
        IDbConnection connection = _connectionFactory.GetOrCreateConnection();

        string sql = $@"
            UPDATE {_tableName}
            SET 
                full_name = @FullName,
                avatar_url = @AvatarUrl
            WHERE user_id = @UserId";

        var command = new CommandDefinition(
            sql,
            parameters: new
            {
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                UserId = user.UserId
            },
            cancellationToken: cancellationToken);

        int rowsAffected = await connection.ExecuteAsync(command);

        return rowsAffected > 0;
    }
}
