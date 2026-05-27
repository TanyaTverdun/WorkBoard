using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WorkBoard.Database.Factories;

namespace WorkBoard.Database.Services;

public class MigrationRunner
{
    private readonly ILogger<MigrationRunner> _logger;
    private const string ConnectionStringKey = "DefaultConnection";
    private const string AppSettingsFileName = "appsettings.json";

    public MigrationRunner(ILogger<MigrationRunner> logger)
    {
        this._logger = logger;
    }

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
                $"Connection string '{ConnectionStringKey}' " +
                $"is missing in {AppSettingsFileName}.");
            return -1;
        }

        DatabaseInitializer.Initialize(connectionString);

        this._logger.LogInformation("Database migrations completed successfully!");

        return 0;
    }
}
