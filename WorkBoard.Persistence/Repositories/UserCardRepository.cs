using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class UserCardRepository
    : GenericRepository<UserCard, (Guid, Guid)>, IUserCardRepository
{
    public UserCardRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal UserCardRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IEnumerable<User>> GetAssigneesByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                u.UserId AS Id,
                u.FullName,
                u.Email,
                u.AvatarUrl
            FROM 
                UserCards uc
            INNER JOIN 
                Users u ON uc.UserId = u.UserId
            WHERE 
                uc.CardId = @CardId;";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                CardId = cardId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryAsync<User>(command);
    }

    public async Task<bool> IsAssignedAsync(
        Guid cardId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM UserCards 
                    WHERE CardId = @CardId AND UserId = @UserId
                ) THEN 1 
                ELSE 0 
            END;";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                CardId = cardId, UserId = userId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task AddAssigneeAsync(
        Guid cardId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO UserCards (CardId, UserId)
            VALUES (@CardId, @UserId);";

        var command = new CommandDefinition(
            sql,
            new 
            { 
                CardId = cardId, UserId = userId 
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }

    public async Task<IReadOnlyList<User>> GetAssignableUsersAsync(
        Guid boardId,
        Guid cardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                u.UserId AS Id,
                u.FullName,
                u.Email,
                u.AvatarUrl
            FROM 
                Users u
            INNER JOIN 
                BoardMembers bm ON u.UserId = bm.UserId
            WHERE 
                bm.BoardId = @BoardId
            ORDER BY 
                u.FullName;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                CardId = cardId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return (await _connection.QueryAsync<User>(command)).ToList();
    }

    public async Task DeleteAssigneeAsync(
        Guid cardId,
        Guid userId,
        CancellationToken cancellation = default)
    {
        const string sql = @"
            DELETE FROM 
                UserCards
            WHERE 
                CardId = @CardId AND 
                UserId = @UserId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                CardId = cardId,
                UserId = userId
            },
            transaction: _transaction,
            cancellationToken: cancellation);

        await _connection.ExecuteAsync(command);
    }
}
