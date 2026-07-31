using System.Text.Json.Serialization;

namespace ArchivationFunctionApp.DTOs;

public class CardArchiveDto
{
    public Guid CardId { get; set; }
    [JsonIgnore] 
    public Guid SectionId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public double Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public List<Guid> AssignedUserIds { get; set; } = new();
    public List<Guid> LabelIds { get; set; } = new();

    public List<ActivityLogArchiveDto> ActivityLogs { get; set; } = new();
    public List<AttachmentArchiveDto> Attachments { get; set; } = new();
    public List<CommentArchiveDto> Comments { get; set; } = new();
    public ChecklistArchiveDto? Checklist { get; set; }
}
