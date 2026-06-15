using Dapper;
using System.Data;
using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class SectionRepository : GenericRepository<Section, Guid>, ISectionRepository
{
    public SectionRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal SectionRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<IReadOnlyList<SectionDto>> GetByBoardIdAsync(
        Guid boardId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT 
                s.SectionId AS Id,
                s.BoardId,
                s.Name,
                s.Position
            FROM 
                Sections s
            JOIN 
                BoardMembers bm 
                ON s.BoardId = bm.BoardId
            WHERE 
                s.BoardId = @BoardId
                AND bm.UserId = @UserId
            ORDER BY 
                s.Position ASC;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                UserId = userId
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        var sections = await _connection.QueryAsync<SectionDto>(command);

        return sections.ToList().AsReadOnly();
    }
}
