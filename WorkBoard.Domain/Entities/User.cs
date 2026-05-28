namespace WorkBoard.Domain.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public required string Email { get; set; }
    public string? AvatarUrl {get; set;}
}
