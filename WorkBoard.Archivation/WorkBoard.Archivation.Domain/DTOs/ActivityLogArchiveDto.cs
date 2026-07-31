using System.Text.Json.Serialization;

namespace WorkBoard.Archivation.Domain.DTOs;

public class ActivityLogArchiveDto
{
    public Guid ActivityLogId { get; set; }
    public Guid UserId { get; set; }
    [JsonIgnore] 
    public Guid CardId { get; set; }
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}
