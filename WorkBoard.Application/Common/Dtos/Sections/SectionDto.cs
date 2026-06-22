namespace WorkBoard.Application.Common.Dtos.Section;

public class SectionDto
{
    public Guid Id { get; set; }
    public Guid BoardId { get; set; }
    public required string Name { get; set; }
    public double Position { get; set; }
}
