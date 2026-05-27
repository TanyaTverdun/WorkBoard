using Microsoft.Extensions.Logging;
using WorkBoard.Database.Exceptions;
using WorkBoard.Database.Factories;
using WorkBoard.Database.Services;

namespace WorkBoard.Database;

/// <summary>
/// The entry point of the database migration
/// </summary>
public class Program
{
    /// <summary>
    /// Starts the application, initializes logging, 
    /// and invokes the migration runner.
    /// </summary>
    /// <param name="args">Command-line arguments</param>
    /// <returns>Returns <c>0</c> on success; 
    /// otherwise, <c>-1</c> if any error occurs.</returns>
    static int Main(string[] args)
    {
        using var loggerFactory = InfrastructureFactory.CreateLoggerFactory();
        var programLogger = loggerFactory.CreateLogger<Program>();

        try
        {
            var runnerLogger = loggerFactory.CreateLogger<MigrationRunner>();
            var runner = new MigrationRunner(runnerLogger);

            return runner.Run();
        }
        catch (DatabaseMigrationException ex)
        {
            programLogger.LogError(
                ex, 
                "Migration failed during DbUp execution. Message: {Message}",
                ex.Message);
            return -1;
        }
        catch (Exception ex)
        {
            programLogger.LogCritical(
                ex, 
                "An unexpected error occurred during migration startup.");
            return -1;
        }
    }
}
