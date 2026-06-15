using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Domain.Entities;

[Table("Boards")]
public class Board : BaseEntity<Guid>
{
    [Column("BoardId")]
    public override Guid Id { get; set; }

    public Guid WorkspaceId { get; set; }

    public required string Name { get; set; }

    public bool IsArchived { get; set; } = false;

    public BoardArchiveStatus ArchiveStatus { get; set; } = 
        BoardArchiveStatus.Active;

    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}
