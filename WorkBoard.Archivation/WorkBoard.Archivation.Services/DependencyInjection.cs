using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;
using WorkBoard.Archivation.Services.Abstractions.Interfaces;
using WorkBoard.Archivation.Services.Options;
using WorkBoard.Archivation.Services.Services;

namespace WorkBoard.Archivation.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CosmosDbOptions>(
            configuration.GetSection(CosmosDbOptions.SectionName));

        services.AddSingleton(sp =>
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

        services.Configure<BlobStorageOptions>(configuration.GetSection(BlobStorageOptions.SectionName));
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BlobStorageOptions>>().Value;
            return new BlobServiceClient(options.ConnectionString);
        });
        services.AddScoped<IBlobArchivationService, BlobArchivationService>();
        services.AddScoped<IArchivationTrackerService, CosmosDbTrackerService>();

        services.Configure<BackendApiOptions>(
             configuration.GetSection(BackendApiOptions.SectionName));

        services.AddRefitClient<IInternalBackendApi>()
            .ConfigureHttpClient((sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<BackendApiOptions>>().Value;

                if (string.IsNullOrWhiteSpace(options.Url))
                {
                    throw new InvalidOperationException("Backend API URL is not configured.");
                }

                c.BaseAddress = new Uri(options.Url);
            });

        return services;
    }
}
