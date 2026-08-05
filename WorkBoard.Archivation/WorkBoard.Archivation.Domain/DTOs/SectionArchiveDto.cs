namespace WorkBoard.Archivation.Domain.DTOs;

public class SectionArchiveDto
{
    public Guid SectionId { get; set; }
    public Guid BoardId { get; set; }
    public required string Name { get; set; }
    public double Position { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public List<CardArchiveDto> Cards { get; set; } = new();
}
