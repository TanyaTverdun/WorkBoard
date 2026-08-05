using System.Text.Json.Serialization;

namespace WorkBoard.Archivation.Domain.DTOs;

public class ChecklistArchiveDto
{
    public Guid ChecklistId { get; set; }
    [JsonIgnore] 
    public Guid CardId { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public List<ChecklistItemArchiveDto> Items { get; set; } = new();
}
