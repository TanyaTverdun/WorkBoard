using Microsoft.Extensions.DependencyInjection;
using WorkBoard.Application.Common.Interfaces;
using WorkBoard.Persistence.Data;
using WorkBoard.Persistence.Repositories;
using WorkBoard.Persistence.UnitOfWork;

namespace WorkBoard.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistance(
        this IServiceCollection services)
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped(
            typeof(IGenericRepository<,>), 
            typeof(GenericRepository<,>));

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        return services;
    }
}
