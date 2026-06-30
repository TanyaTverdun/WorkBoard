namespace WorkBoard.Infrastructure.Options;

public class AzureOptions
{
    public const string SectionName = "Azure";

    public SignalROptions SignalR { get; set; } = new();
}
