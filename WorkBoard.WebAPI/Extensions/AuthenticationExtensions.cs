using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace WorkBoard.WebAPI.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomJwtChallengeResponse(
        this IServiceCollection services)
    {
        services.Configure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme, 
            options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 
                            StatusCodes.Status401Unauthorized;

                        context.Response.ContentType = "application/json";

                        var problemDetails = new ProblemDetails
                        {
                            Title = "Unauthorized access.",
                            Status = StatusCodes.Status401Unauthorized,
                            Detail = 
                                "You must be authenticated or have correct " +
                                "permissions to access this resource.",
                            Instance = context.Request.Path
                        };

                        var json = JsonSerializer.Serialize(problemDetails);
                        await context.Response.WriteAsync(json);
                    }
                };
            });

        return services;
    }
}
