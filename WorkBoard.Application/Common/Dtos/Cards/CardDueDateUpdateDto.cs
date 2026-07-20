namespace WorkBoard.Application.Common.Dtos.Cards;

public class CardDueDateUpdateDto
{
    public Guid CardId { get; set; }
    public DateTime? DueDate { get; set; }
}
