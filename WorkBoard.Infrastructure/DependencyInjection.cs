using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using WorkBoard.Application.Common.Constants;
using WorkBoard.Application.Common.Interfaces.BlobStorage;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Database.Options;
using WorkBoard.Infrastructure.AI.Plugins;
using WorkBoard.Infrastructure.BlobStorage;
using WorkBoard.Infrastructure.Options;
using WorkBoard.Infrastructure.SignalR.Services;
using WorkBoard.WebAPI.Constants;

namespace WorkBoard.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databaseOptions = configuration
            .GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>()
                ?? throw new InvalidOperationException(
                    "Database section is missing in appsettings.json");

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
                databaseOptions.ConnectionString,
                new SqlServerStorageOptions
                {
                    SchemaName = ServiceBusOptions.HangfireSchema,
                    PrepareSchemaIfNecessary = true,
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true
                }));

        services.AddHangfireServer();

        services.Configure<AzureOpenAIOptions>(
            configuration.GetSection(AzureOpenAIOptions.SectionName));

        services.Configure<AzureAISearchOptions>(
            configuration.GetSection(AzureAISearchOptions.SectionName));

        var openAiOptions = configuration
            .GetSection(AzureOpenAIOptions.SectionName)
            .Get<AzureOpenAIOptions>()
                ?? throw new InvalidOperationException(
                    $"Section '{AzureOpenAIOptions.SectionName}' " +
                    $"is missing in appsettings.json");

        var aiSearchOptions = configuration
            .GetSection(AzureAISearchOptions.SectionName)
            .Get<AzureAISearchOptions>()
                ?? throw new InvalidOperationException(
                    $"Section '{AzureAISearchOptions.SectionName}' " +
                    $"is missing in appsettings.json");

        var embeddingConfig = new AzureOpenAIConfig
        {
            APIKey = openAiOptions.ApiKey,
            Deployment = openAiOptions.EmbeddingModelDeploymentName,
            Endpoint = openAiOptions.Endpoint,
            APIType = AzureOpenAIConfig.APITypes.EmbeddingGeneration,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        };

        var chatConfig = new AzureOpenAIConfig
        {
            APIKey = openAiOptions.ApiKey,
            Deployment = openAiOptions.TextModelDeploymentName,
            Endpoint = openAiOptions.Endpoint,
            APIType = AzureOpenAIConfig.APITypes.ChatCompletion,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        };

        var memory = new KernelMemoryBuilder()
            .WithAzureOpenAITextGeneration(chatConfig)
            .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
            .WithAzureAISearchMemoryDb(aiSearchOptions.Endpoint, aiSearchOptions.ApiKey)
            .WithAzureBlobsDocumentStorage(new AzureBlobsConfig
            {
                Auth = AzureBlobsConfig.AuthTypes.ConnectionString,
                ConnectionString = azureOptions.BlobStorage.ConnectionString,
                Container = BlobContainers.KernelMemoryDocs
            })
            .Build<MemoryServerless>();

        services.AddSingleton<IKernelMemory>(memory);

        services.AddTransient<KnowledgeBasePlugin>();

        services.AddTransient<Kernel>(sp =>
        {
            var builder = Kernel.CreateBuilder();

            builder.AddAzureOpenAIChatCompletion(
                deploymentName: openAiOptions.TextModelDeploymentName,
                endpoint: openAiOptions.Endpoint,
                apiKey: openAiOptions.ApiKey
            );

            var knowledgeBasePlugin = sp.GetRequiredService<KnowledgeBasePlugin>();

            builder.Plugins.AddFromObject(
                knowledgeBasePlugin, 
                KnowledgeBaseConstants.PluginName);

            return builder.Build();
        });

        services.AddTransient<IBoardNotificationService, BoardNotificationService>();
        services.AddTransient<IArchivationNotificationService, ArchivationNotificationService>();
        services.AddTransient<IAppNotificationService, AppNotificationService>();
        services.AddTransient<IWorkspaceNotificationService, WorkspaceNotificationService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        return services;
    }
}
