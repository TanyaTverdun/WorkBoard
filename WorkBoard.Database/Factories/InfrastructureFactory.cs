using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WorkBoard.Database.Factories;

/// <summary>
/// Factory for creating configuration and logging.
/// </summary>
public static class InfrastructureFactory
{
    /// <summary>
    /// Loads settings from appsettings.json and appsettings.Development.json.
    /// </summary>
    /// <param name="baseFileName">The main configuration 
    /// file name (usually "appsettings.json").</param>
    /// <returns>A ready-to-use configuration object.</returns>
    public static IConfiguration BuildConfiguration(string baseFileName)
    {
        var environment = Environment
            .GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "Production";

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                baseFileName, 
                optional: false, 
                reloadOnChange: true)
            .AddJsonFile(
                $"appsettings.{environment}.json", 
                optional: true, 
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    /// <summary>
    /// Creates a .NET logger that prints structured logs directly to the console window.
    /// </summary>
    /// <returns>A configured logger factory instance.</returns>
    public static ILoggerFactory CreateLoggerFactory()
    {
        return LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information);
        });
    }
}
