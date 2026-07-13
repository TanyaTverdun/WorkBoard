using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("ActivityLogs")]
public class ActivityLog : BaseEntity<Guid>
{
    [Column("ActivityLogId")]
    public override Guid Id { get; set; }

    public required Guid CardId { get; set; }

    public required Guid UserId { get; set; }

    [NotMapped]
    public string? FullName { get; set; }

    public required string Text { get; set; }

    public required DateTime CreatedAt { get; set; }
}
