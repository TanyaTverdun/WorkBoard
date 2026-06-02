using System.Data;
using Microsoft.Extensions.Options;
using WorkBoard.Database.Options;
using WorkBoard.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace WorkBoard.Persistence.Data;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;
    private IDbConnection? _connection;
    private bool _disposed = false;

    public IDbConnectionFactory(IOptions<DatabaseOptions> options)
    {
        if (options is null ||
            string.IsNullOrWhiteSpace(options.Value.ConnectionString))
        {
            throw new ArgumentException(
                "Database connection string cannot be null or empty.", 
                nameof(options));
        }

        _connectionString = options.Value.ConnectionString;
    }

    public IDbConnection GetOrCreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
