using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using WorkBoard.Application.Common.Behaviours;

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

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}
