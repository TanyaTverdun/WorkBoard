using DbUp;
using System.Reflection;
using WorkBoard.Database.Exceptions;

namespace WorkBoard.Database.Services;

/// <summary>
/// execute embedded SQL migration scripts using the DbUp library.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Ensures the target database exists and 
    /// applies all pending SQL migration scripts.
    /// </summary>
    /// <param name="connectionString">The connection string used 
    /// to connect to the SQL Server instance.</param>
    /// <exception cref="DatabaseMigrationException">Thrown when the DbUp migration 
    /// process fails due to SQL syntax errors or connectivity issues.</exception>
    public static void Initialize(string connectionString)
    {
        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new DatabaseMigrationException(
                "Database migration failed during DbUp execution", 
                result.Error);
        }
    }
}
