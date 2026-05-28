namespace WorkBoard.Database.Options
{
    /// <summary>
    /// Represents the configuration options for the database
    /// </summary>
    public class DatabaseOptions
    {
        /// <summary>
        /// The default section name in the configuration
        /// </summary>
        public const string SectionName = "Database";

        /// <summary>
        /// Gets or sets the database connection string
        /// </summary>
        public required string ConnectionString { get; set; } = string.Empty;
    }
}
