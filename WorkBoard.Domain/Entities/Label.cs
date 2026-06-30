using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Labels")]
public class Label : BaseEntity<Guid>
{
    [Column("LabelId")]
    public override Guid Id { get; set; }

    public Guid BoardId { get; set; }

    public required string Name { get; set; }

    public string? Color { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
