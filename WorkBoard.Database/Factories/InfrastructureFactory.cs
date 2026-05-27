using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WorkBoard.Database.Factories;

public static class InfrastructureFactory
{
    public static IConfiguration BuildConfiguration(string fileName)
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                fileName, 
                optional: false, 
                reloadOnChange: true)
            .Build();
    }

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
