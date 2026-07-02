namespace WorkBoard.Application.Common.Dtos.Checklists;

public class ChecklistItemDto
{
    public Guid ChecklistId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
}
