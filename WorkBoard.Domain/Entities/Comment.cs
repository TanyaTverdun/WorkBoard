using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Coments")]
public class Comment : BaseEntity<Guid>
{
    [Column("ComentId")]
    public override Guid Id { get; set; }

    public required Guid CardId { get; set; }

    public required Guid UserId { get; set; }

    public required string Text { get; set; }

    public DateTime CreatedAt { get; set; }
}
