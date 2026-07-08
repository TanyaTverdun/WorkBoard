namespace WorkBoard.Application.Common.Dtos.Attachments;

public class AttachmentDto
{
    public Guid Id { get; set; }

    public Guid CardId { get; set; }

    public required string FileUrl { get; set; }

    public required string FileName { get; set; }

    public long FileSizeBytes { get; set; }
}
