using WorkBoard.Application.Common.Interfaces;
using WorkBoard.WebAPI.Extensions;
using WorkBoard.WebAPI.Services;

namespace WorkBoard.WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();

        services.AddSwaggerWithJwtAuth();

        services.AddCustomJwtChallengeResponse();

        return services;
    }
}
