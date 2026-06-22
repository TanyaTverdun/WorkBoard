using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.Users;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class UserRepository : GenericRepository<User, Guid>, IUserRepository
{
    public UserRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal UserRepository(IDbConnection connection, IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<User?> GetByIdOrEmailAsync(
    Guid id,
    string? email,
    CancellationToken cancellationToken = default)
    {
        const string sql = @"
        SELECT TOP 1
            UserId AS Id,
            FullName,
            Email,
            AvatarUrl
        FROM 
            Users
        WHERE 
            UserId = @Id 
            OR (@Email IS NOT NULL AND 
                LOWER(Email) = LOWER(@Email));";

        var command = new CommandDefinition(
            sql,
            new
            {
                Id = id,
                Email = email
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<User>(command);
    }

    public async Task<IReadOnlyList<UserSearchDto>> SearchAssignableUsersAsync(
        Guid boardId,
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT TOP 10
                UserId,
                FullName,
                Email,
                AvatarUrl
            FROM 
                Users
            WHERE 
                Email LIKE @SearchTerm + '%'
                AND UserId NOT IN (
                    SELECT 
                        UserId 
                    FROM 
                        BoardMembers 
                    WHERE 
                        BoardId = @BoardId
                )
            ORDER BY 
                Email;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                SearchTerm = searchTerm
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var users = await _connection.QueryAsync<UserSearchDto>(command);

        return users.ToList().AsReadOnly();
    }
}
