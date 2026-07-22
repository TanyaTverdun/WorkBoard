namespace WorkBoard.Application.Common.Models;

public class ActivityLogModel
{
    public Guid Id { get; set; }

    public required Guid CardId { get; set; }

    public required Guid UserId { get; set; }

    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }

    public string? AvatarColor { get; set; }

    public required string Text { get; set; }

    public required DateTime CreatedAt { get; set; }
}
