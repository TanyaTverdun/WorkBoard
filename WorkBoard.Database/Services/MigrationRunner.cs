using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkBoard.Database.Factories;

namespace WorkBoard.Database.Services;

/// <summary>
/// Orchestrates the database migration process by loading configuration,
/// validating the connection string, and executing the database initializer.
/// </summary>
public class MigrationRunner
{
    private readonly ILogger<MigrationRunner> _logger;
    private const string ConnectionStringKey = "DefaultConnection";
    private const string AppSettingsFileName = "appsettings.json";

    public MigrationRunner(ILogger<MigrationRunner> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// Runs the database migration process.
    /// </summary>
    /// <returns>
    /// Returns <c>0</c> if the migration completed successfully; 
    /// otherwise, returns <c>-1</c> if a configuration error occurs.
    /// </returns>
    public int Run()
    {
        this._logger.LogInformation("Starting database migration");

        var configuration = InfrastructureFactory
            .BuildConfiguration(AppSettingsFileName);
        var connectionString = configuration
            .GetConnectionString(ConnectionStringKey);

        if (string.IsNullOrEmpty(connectionString))
        {
            this._logger.LogError(
                "Connection string '{ConnectionStringKey}' " +
                "is missing in {AppSettingsFileName}.",
                ConnectionStringKey, AppSettingsFileName);
            return -1;
        }

        DatabaseInitializer.Initialize(connectionString);

        this._logger.LogInformation("Database migrations completed successfully!");

        return 0;
    }
}
