using Microsoft.Identity.Web;
using WorkBoard.Application;
using WorkBoard.Database;
using WorkBoard.Database.Options;
using WorkBoard.Infrastructure;
using WorkBoard.Infrastructure.SignalR.Hubs;
using WorkBoard.Persistence;
using WorkBoard.WebAPI;
using WorkBoard.WebAPI.Constants;
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

builder.Services.AddWebApiServices(builder.Configuration);
builder.Services.AddPersistance();
builder.Services.AddApplication();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>();
    await initializer.Initialize();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkBoard API v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(WorkBoard.WebAPI.DependencyInjection.BlazorWasmPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BoardHub>("/hubs/board");

app.Run();
