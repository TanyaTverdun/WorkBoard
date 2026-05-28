using Microsoft.Extensions.DependencyInjection;
using WorkBoard.Application.Common.Interfaces;

namespace WorkBoard.Persistence;

/// <summary>
/// Extension methods for registering persistence 
/// layer services into DI container
/// </summary>
public static class DependencyInjection
{

    /// <summary>
    /// Registers persistence services
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add services
    /// </param>
    /// <returns>
    /// The same service collection
    /// </returns>
    public static IServiceCollection AddPersistance(
        this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, IDbConnectionFactory>();

        return services;
    }
}
