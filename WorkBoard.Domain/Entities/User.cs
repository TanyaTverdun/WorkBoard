using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Users")]
public class User : BaseEntity<Guid>
{
    [Key]
    [Column("UserId")]
    public override Guid Id { get; set; }
    public string? FullName { get; set; }
    public required string Email { get; set; }
    public string? AvatarUrl {get; set;}
}
