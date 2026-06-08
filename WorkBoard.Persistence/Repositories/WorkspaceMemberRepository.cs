using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class WorkspaceMemberRepository : IWorkspaceMemberRepository
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction? _transaction;

    public WorkspaceMemberRepository(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.Create();
        _transaction = null;
    }

    internal WorkspaceMemberRepository(
        IDbConnection connection, 
        IDbTransaction transaction)
    {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddMemberAsync(
        WorkspaceMember member, 
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO WorkspaceMembers (
                UserId, 
                WorkspaceId, 
                UserRole)
            VALUES (
                @UserId, 
                @WorkspaceId, 
                @UserRole);";

        var command = new CommandDefinition(
            sql,
            member,
            transaction: _transaction,
            cancellationToken: cancellationToken);

        await _connection.ExecuteAsync(command);
    }
}
