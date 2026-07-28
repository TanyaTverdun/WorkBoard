using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Infrastructure.BlobStorage;
using WorkBoard.Infrastructure.Options;
using WorkBoard.Infrastructure.SignalR.Services;

namespace WorkBoard.Infrastructure;

public static class DependencyInjection
{
    private const string DefaultConnection = "DefaultConnection";
    private const string HangfireSchema = "hangfire";

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AzureOptions>(
            configuration.GetSection(AzureOptions.SectionName));

        var azureOptions = configuration
            .GetSection(AzureOptions.SectionName)
            .Get<AzureOptions>()
                ?? throw new InvalidOperationException(
                    "Azure section is missing in appsettings.json");

        if (string.IsNullOrEmpty(azureOptions.SignalR?.ConnectionString))
        {
            throw new InvalidOperationException(
                "Azure SignalR Connection String is missing in appsettings.json");
        }

        services.AddSignalR()
                .AddAzureSignalR(azureOptions.SignalR.ConnectionString);

        if (string.IsNullOrEmpty(azureOptions.BlobStorage?.ConnectionString))
        {
            throw new InvalidOperationException(
                "Azure Blob Storage Connection String is missing in appsettings.json");
        }

        if (string.IsNullOrEmpty(azureOptions.ServiceBus?.ConnectionString))
        {
            throw new InvalidOperationException(
                "Azure Service Bus Connection String is missing in appsettings.json");
        }

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddBlobServiceClient(azureOptions.BlobStorage.ConnectionString);
            clientBuilder.AddServiceBusClient(azureOptions.ServiceBus.ConnectionString);
        });

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(
                configuration.GetConnectionString(DefaultConnection), 
                new SqlServerStorageOptions
                {
                    SchemaName = HangfireSchema,
                    PrepareSchemaIfNecessary = true,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true
                }));

        services.AddHangfireServer();

        services.AddTransient<IBoardNotificationService, BoardNotificationService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        return services;
    }
}
