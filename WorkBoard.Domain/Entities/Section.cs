using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Sections")]
public class Section : BaseEntity<Guid>
{
    [Column("SectionId")]
    public override Guid Id { get; set; }

    public Guid BoardId { get; set; }

    public required string Name { get; set; }

    public double Position { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
