using ArchivationFunctionApp.DataAccess;
using ArchivationFunctionApp.Interfaces;
using ArchivationFunctionApp.Options;
using ArchivationFunctionApp.Repositories;
using ArchivationFunctionApp.Services;
using Azure.Monitor.OpenTelemetry.Exporter;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));

builder.Services.Configure<BlobStorageOptions>(
    builder.Configuration.GetSection(BlobStorageOptions.SectionName));

builder.Services.Configure<CosmosDbOptions>(
    builder.Configuration.GetSection(CosmosDbOptions.SectionName));

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<BlobStorageOptions>>().Value;
    return new BlobServiceClient(options.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<CosmosDbOptions>>().Value;

    var cosmosClientOptions = new CosmosClientOptions
    {
        HttpClientFactory = () =>
        {
            var httpMessageHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            return new HttpClient(httpMessageHandler);
        },
        ConnectionMode = ConnectionMode.Gateway
    };

    return new CosmosClient(options.ConnectionString, cosmosClientOptions);
});

builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IBoardArchiveRepository, BoardArchiveRepository>();
builder.Services.AddScoped<IArchivationTrackerService, CosmosDbTrackerService>();

builder.Services.AddOpenTelemetry()
    .UseFunctionsWorkerDefaults()
    .UseAzureMonitorExporter();

builder.Build().Run();
