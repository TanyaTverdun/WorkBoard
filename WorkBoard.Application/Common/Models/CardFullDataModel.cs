using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Models;

public class CardFullDataModel
{
    public Card Card { get; set; } = null!;
    public List<User> Assignees { get; set; } = new();
    public List<Label> Labels { get; set; } = new();
    public Checklist? Checklist { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<ActivityLog> ActivityLogs { get; set; } = new();
}
