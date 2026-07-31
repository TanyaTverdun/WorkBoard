using System.Text.Json.Serialization;

namespace ArchivationFunctionApp.DTOs;

public class ChecklistItemArchiveDto
{
    public Guid ChecklistItemId { get; set; }
    [JsonIgnore] 
    public Guid ChecklistId { get; set; }
    public required string Title { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
