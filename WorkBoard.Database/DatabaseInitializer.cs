using DbUp;
using WorkBoard.Database.Exceptions;

namespace WorkBoard.Database;

/// <summary>
/// execute embedded SQL migration scripts using the DbUp library.
/// </summary>
public class DatabaseInitializer
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="DatabaseInitializer"/> class.
    /// </summary>
    /// <param name="connectionString">
    /// The connection string used to 
    /// connect to the SQL Server instance.
    /// </param>
    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Ensures the target database exists and 
    /// applies all pending SQL migration scripts.
    /// </summary>
    /// <exception cref="DatabaseMigrationException">T
    /// hrown when the DbUp migration 
    /// process fails due to SQL syntax errors or connectivity issues.
    /// </exception>
    public async Task Initialize()
    {
        EnsureDatabase.For.SqlDatabase(_connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly)
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new DatabaseMigrationException(
                    "Database migration failed during DbUp execution",
                    result.Error);
            }
        }
    }
}
