using ArchivationFunctionApp.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkBoard.Archivation.DataAccess.Abstractions.Interfaces;
using WorkBoard.Archivation.DataAccess.Options;

namespace WorkBoard.Archivation.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(
            configuration.GetSection(DatabaseOptions.SectionName));

        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IBoardArchiveRepository, BoardArchiveRepository>();

        return services;
    }
}
