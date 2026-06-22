using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.BoardMembers;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

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

    public async Task<IReadOnlyList<BoardMemberWithUserDto>> GetMembersByBoardAsync(
        Guid boardId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                bm.UserId, 
                bm.BoardId, 
                bm.UserRole,
                u.UserId AS Id, 
                u.FullName, 
                u.Email, 
                u.AvatarUrl 
            FROM 
                BoardMembers bm
            JOIN 
                Users u ON bm.UserId = u.UserId
            WHERE 
                bm.BoardId = @BoardId;";

        var command = new CommandDefinition(
            sql,
            new { BoardId = boardId },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var result = await _connection
            .QueryAsync<BoardMember, User, BoardMemberWithUserDto>(
                command,
                (member, user) => new BoardMemberWithUserDto(member, user),
                splitOn: "Id");

        return result.ToList().AsReadOnly();
    }

    public async Task<int> UpdateRoleAsync(
        Guid boardId,
        Guid userId,
        BoardRole newRole,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE 
                BoardMembers
            SET 
                UserRole = @NewRole
            WHERE 
                BoardId = @BoardId 
                AND UserId = @UserId;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                UserId = userId,
                NewRole = (int)newRole
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteAsync(command);
    }

    public async Task AddAsync(
        Guid boardId,
        Guid userId,
        BoardRole role,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO BoardMembers (
                BoardId, 
                UserId, 
                UserRole)
            VALUES (
                @BoardId, 
                @UserId, 
                @UserRole);";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                UserId = userId,
                UserRole = (int)role
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }
}
