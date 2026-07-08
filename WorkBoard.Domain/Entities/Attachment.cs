using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("Attachments")]
public class Attachment : BaseEntity<Guid>
{
    [Column("AttachmentId")]
    public override Guid Id { get; set; }

    public required Guid CardId { get; set; }

    public required string FileUrl { get; set; }

    public required string FileName { get; set; }

    public required long FileSizeBytes { get; set; }
}
