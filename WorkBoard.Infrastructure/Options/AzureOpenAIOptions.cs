namespace WorkBoard.Infrastructure.Options;

public class AzureOpenAIOptions
{
    public const string SectionName = "Azure:OpenAI";

    public required string Endpoint { get; set; }
    public required string ApiKey { get; set; }
    public required string TextModelDeploymentName { get; set; }
    public required string EmbeddingModelDeploymentName { get; set; }
}
