using System.Data;
using Microsoft.Extensions.Options;
using WorkBoard.Database.Options;
using WorkBoard.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace WorkBoard.Persistence.Data;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _connectionString = options?.Value?.ConnectionString ?? 
            throw new ArgumentException(nameof(options));
    }

    public IDbConnection Create()
    {
        return new SqlConnection(_connectionString);
    }
}
