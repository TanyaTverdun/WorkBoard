using Microsoft.Identity.Web;
using WorkBoard.Database;
using WorkBoard.Database.Options;
using WorkBoard.WebAPI.Constants;
using WorkBoard.WebAPI.Extensions;
using WorkBoard.WebAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<DatabaseOptions>(
    builder.Configuration.GetSection(DatabaseOptions.SectionName));

builder.Services.AddTransient<DatabaseInitializer>();

builder.Services.AddMicrosoftIdentityWebApiAuthentication(
    builder.Configuration,
    ConfigurationSections.AzureAd);

builder.Services.AddSwaggerWithJwtAuth();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>();
    await initializer.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
