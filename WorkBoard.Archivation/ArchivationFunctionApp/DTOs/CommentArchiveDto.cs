using System.Text.Json.Serialization;

namespace ArchivationFunctionApp.DTOs;

public class CommentArchiveDto
{
    public Guid ComentId { get; set; }
    public Guid UserId { get; set; }
    [JsonIgnore] 
    public Guid CardId { get; set; }
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; }
}
