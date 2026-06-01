using Microsoft.OpenApi;

namespace WorkBoard.WebAPI.Extensions;

public static class SwaggerExtensions
{
    private const string ApiVersion = "v1";
    private const string ApiTitle = "WorkBoard API";
    private const string SecuritySchemeId = "Bearer";
    private const string SecuritySchemeName = "Authorization";
    private const string BearerFormat = "JWT";

    public static IServiceCollection AddSwaggerWithJwtAuth(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(ApiVersion, new OpenApiInfo
            {
                Title = ApiTitle,
                Version = ApiVersion
            });

            options.AddSecurityDefinition(
                SecuritySchemeId, 
                new OpenApiSecurityScheme
                {
                    Name = SecuritySchemeName,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = SecuritySchemeId,
                    BearerFormat = BearerFormat,
                    In = ParameterLocation.Header,
                    Description = 
                        "Please enter token in format: Bearer {your_token}"
                });

            options.AddSecurityRequirement(document => new()
            {
                [new OpenApiSecuritySchemeReference(
                    SecuritySchemeId, 
                    document)] = new List<string>()
            });
        });

        return services;
    }
}
