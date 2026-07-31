using ArchivationFunctionApp.Interfaces;
using ArchivationFunctionApp.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace ArchivationFunctionApp.DataAccess;

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
