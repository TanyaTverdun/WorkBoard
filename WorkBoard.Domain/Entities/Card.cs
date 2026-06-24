using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Cards")]
public class Card : BaseEntity<Guid>
{
    [Column("CardId")]
    public override Guid Id { get; set; }

    public required Guid SectionId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public required double Position { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
}
