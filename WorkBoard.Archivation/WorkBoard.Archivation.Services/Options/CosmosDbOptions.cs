namespace WorkBoard.Archivation.Services.Options;

public class CosmosDbOptions
{
    public const string SectionName = "CosmosDb";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
}
