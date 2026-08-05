namespace WorkBoard.Archivation.Domain.DTOs;

public class BoardArchiveDto
{
    public Guid BoardId { get; set; }
    public Guid WorkspaceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public List<BoardMemberArchiveDto> Members { get; set; } = new();
    public List<LabelArchiveDto> Labels { get; set; } = new();
    public List<SectionArchiveDto> Sections { get; set; } = new();
}
