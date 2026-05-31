using System.Data;
using Microsoft.Extensions.Options;
using WorkBoard.Database.Options;
using WorkBoard.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;

namespace WorkBoard.Persistence.Data;

/// <summary>
/// Connection factori for Microsoft SQL Server
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory, IDisposable
{
    private readonly string _connectionString;
    private IDbConnection? _connection;
    private bool _disposed = false;


    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionFactory"/> class.
    /// </summary>
    /// <param name="options">
    /// The database configuration options
    /// </param>
    /// <exception cref="ArgumentException">
    /// thow exception when <param name="options"> is null or white space
    /// </exception>
    public DbConnectionFactory(IOptions<DatabaseOptions> options)
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

    /// <inheritdoc />
    public IDbConnection GetOrCreateConnection()
    {
        if (_connection is null ||
            _connection.State == ConnectionState.Closed)
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        return _connection;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Dispose();
                _connection = null;
            }

            _disposed = true;
        }
    }
}
