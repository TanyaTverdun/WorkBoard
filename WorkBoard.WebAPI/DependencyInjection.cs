using WorkBoard.Application.Common.Interfaces;
using WorkBoard.WebAPI.Constants;
using WorkBoard.WebAPI.Extensions;
using WorkBoard.WebAPI.Services;

namespace WorkBoard.WebAPI;

public static class DependencyInjection
{
    public const string BlazorWasmPolicyName = "BlazorWasmPolicy";

    public static IServiceCollection AddWebApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IUserContext, UserContext>();

        services.AddSwaggerWithJwtAuth();

        services.AddCustomJwtChallengeResponse();

        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.SectionName));

        services.AddCors(options =>
        {
            var corsOptions = configuration.GetSection(
                CorsOptions.SectionName).Get<CorsOptions>()
                    ?? new CorsOptions();

            options.AddPolicy(BlazorWasmPolicyName, policy =>
            {
                policy.WithOrigins(corsOptions.AllowedOrigins)
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        return services;
    }
}
