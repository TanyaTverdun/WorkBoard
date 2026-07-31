using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkBoard.Archivation.DataAccess;
using WorkBoard.Archivation.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .UseFunctionsWorkerDefaults()
    .UseAzureMonitorExporter();

builder.Build().Run();
