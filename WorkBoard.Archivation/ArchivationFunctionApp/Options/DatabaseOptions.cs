namespace ArchivationFunctionApp.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string ConnectionString { get; set; }
}
