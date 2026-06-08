using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Domain.Entities;

[Table("Workspaces")]
public class Workspace : BaseEntity<Guid>
{
    [Column("WorkspaceId")]
    public override Guid Id { get; set; }

    public required string Name { get; set; }

    public SubscriptionTier SubscriptionTier { get; set; } = 
        SubscriptionTier.Free;

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
