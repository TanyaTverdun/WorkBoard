namespace WorkBoard.Infrastructure.Options;

public class AzureAISearchOptions
{
    public const string SectionName = "Azure:AISearch";

    public required string Endpoint { get; set; }
    public required string ApiKey { get; set; }
}
