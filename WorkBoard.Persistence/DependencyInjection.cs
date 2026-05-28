using Microsoft.Extensions.DependencyInjection;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Persistence.Data;
using WorkBoard.Persistence.Repositories;
using UnitOfWorkClass = WorkBoard.Persistence.UnitOfWork.UnitOfWork;
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
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWorkClass>();

        return services;
    }
}
