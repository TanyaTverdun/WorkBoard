using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WorkBoard.Application;

/// <summary>
/// Extension methods to register application-layer services 
/// into the dependency injection container
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application services
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance
    /// </returns>
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        });

        return services;
    }
}
