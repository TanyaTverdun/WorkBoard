using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class BoardMemberRepository 
    : GenericRepository<BoardMember, (Guid, Guid)>, IBoardMemberRepository
{
    public BoardMemberRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal BoardMemberRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task AddMemberAsync(
        BoardMember member,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO BoardMembers (
                UserId, 
                BoardId, 
                UserRole)
            VALUES (
                @UserId, 
                @BoardId, 
                @UserRole);";

        var command = new CommandDefinition(
            sql,
            member,
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }

    public async Task<bool> IsMemberAsync(
        Guid boardId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM BoardMembers 
                    WHERE BoardId = @BoardId AND UserId = @UserId
                ) THEN 1 
                ELSE 0 
            END;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                UserId = userId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<BoardMember?> GetMembershipAsync(
        Guid userId,
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                UserId, 
                BoardId, 
                UserRole
            FROM 
                BoardMembers
            WHERE 
                UserId = @UserId 
                AND BoardId = @BoardId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                UserId = userId,
                BoardId = boardId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.QueryFirstOrDefaultAsync<BoardMember>(command);
    }
}
