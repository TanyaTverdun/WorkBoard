using System.Text.Json.Serialization;

namespace ArchivationFunctionApp.DTOs;

public class AttachmentArchiveDto
{
    public Guid AttachmentId { get; set; }
    [JsonIgnore] 
    public Guid CardId { get; set; }
    public required string FileUrl { get; set; }
    public required string FileName { get; set; }
    public long FileSizeBytes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
