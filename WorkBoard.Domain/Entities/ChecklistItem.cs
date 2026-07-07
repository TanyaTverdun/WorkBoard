using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Checklist_items")]
public class ChecklistItem : BaseEntity<Guid>
{
    [Column("ChecklistItemId")]
    public override Guid Id { get; set; }

    public Guid ChecklistId { get; set; }

    public required string Title { get; set; }

    public bool IsDone { get; set; }

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
