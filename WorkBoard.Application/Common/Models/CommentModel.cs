namespace WorkBoard.Application.Common.Models;

public class CommentModel
{
    public Guid Id { get; set; }
    public required Guid CardId { get; set; }
    public required Guid UserId { get; set; }
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserFullName { get; set; }
    public string? UserAvatarUrl { get; set; }
    public required string UserAvatarColor { get; set; }
}
