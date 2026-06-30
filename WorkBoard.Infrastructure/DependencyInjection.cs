using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkBoard.Application.Common.Interfaces.Notification;
using WorkBoard.Infrastructure.Options;
using WorkBoard.Infrastructure.SignalR.Services;

namespace WorkBoard.Infrastructure;

public static class DependencyInjection
{
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

        services.AddTransient<IBoardNotificationService, BoardNotificationService>();

        return services;
    }
}
