namespace ArchivationFunctionApp.DTOs;

public class LabelArchiveDto
{
    public Guid LabelId { get; set; }
    public Guid BoardId { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
