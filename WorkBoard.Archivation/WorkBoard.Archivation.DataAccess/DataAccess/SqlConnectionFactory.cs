using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;
using WorkBoard.Archivation.DataAccess.Abstractions.Interfaces;
using WorkBoard.Archivation.DataAccess.Options;

namespace WorkBoard.Archivation.DataAccess;

public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _options;

    public SqlConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public IDbConnection Create()
    {
        return new SqlConnection(_options.ConnectionString);
    }
}
