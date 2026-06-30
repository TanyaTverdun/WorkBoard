namespace WorkBoard.Application.Common.Dtos.Labels;

public class CreateLabelRequest
{
    public required string Name { get; set; }
    public string? Color { get; set; }
}
