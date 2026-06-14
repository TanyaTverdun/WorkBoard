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
}
