using WorkBoard.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTransient<DatabaseInitializer>(provider =>
{
    var connectionString = builder.Configuration
        .GetConnectionString(ConnectionStringNames.DefaultConnection);

    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException(
            $"Connection string '{ConnectionStringNames.DefaultConnection}' " +
            $"is missing from configuration.");
    }

    return new DatabaseInitializer(connectionString);
});

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
