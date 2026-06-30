using Dapper;
using System.Data;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Application.Common.Interfaces.Repositories;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Persistence.Repositories;

public class LabelRepository : GenericRepository<Label, Guid>, ILabelRepository
{
    public LabelRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory)
    {
    }

    internal LabelRepository(
        IDbConnection connection,
        IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<bool> IsNameUniqueAsync(
        Guid boardId,
        string name,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT CASE 
                WHEN EXISTS (
                    SELECT 1 
                    FROM Labels 
                    WHERE BoardId = @BoardId AND Name = @Name
                ) THEN 0 
                ELSE 1 
            END;";

        var command = new CommandDefinition(
            sql,
            new
            {
                BoardId = boardId,
                Name = name
            },
            transaction: _transaction,
            cancellationToken: cancellationToken);

        return await _connection.ExecuteScalarAsync<bool>(command);
    }
}
