using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Checklists")]
public class Checklist : BaseEntity<Guid>
{
    [Column("ChecklistId")]
    public override Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
