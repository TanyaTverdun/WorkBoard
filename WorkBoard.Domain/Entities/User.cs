using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

public class User : BaseEntity<Guid>
{
    public string? FullName { get; set; }
    public required string Email { get; set; }
    public string? AvatarUrl {get; set;}
}
