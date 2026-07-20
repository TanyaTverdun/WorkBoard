namespace WorkBoard.Application.Common.Models;

public class CardSummaryModel
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public double Position { get; set; }
    public int CommentsCount { get; set; }
    public int AttachmentsCount { get; set; }
    public int ChecklistTotalItems { get; set; }
    public int ChecklistDoneItems { get; set; }
    public List<LabelModel> Labels { get; set; } = new();
    public List<AssigneeModel> Assignees { get; set; } = new();
}
