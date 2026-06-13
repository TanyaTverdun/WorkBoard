using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.Board;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class BoardRepository :
GenericRepository<Board, Guid>, IBoardRepository
{
    public BoardRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal BoardRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<BoardDto>> GetByWorkspaceIdAsync(
        Guid workspaceId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                b.BoardId AS Id,
                b.WorkspaceId,
                b.Name,
                bm.UserRole
            FROM 
                Boards b
            JOIN 
                BoardMembers bm 
                ON b.BoardId = bm.BoardId
            WHERE 
                b.WorkspaceId = @WorkspaceId
                AND bm.UserId = @UserId
                AND b.IsArchived = 0
            ORDER BY 
                b.CreatedAt DESC;";

        var command = new CommandDefinition(
            sql,
            new
            {
                WorkspaceId = workspaceId,
                UserId = userId
            },
            cancellationToken: cancellationToken);

        var boards = await _connection.QueryAsync<BoardDto>(command);

        return boards.ToList().AsReadOnly();
    }
}
