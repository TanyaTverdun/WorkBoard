namespace WorkBoard.Infrastructure.Options;

public class ServiceBusOptions
{
    public const string HangfireSchema = "hangfire";

    public string ConnectionString { get; set; } = string.Empty;
}
