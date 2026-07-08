using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.UpdateCardDueDate;

public class UpdateCardDueDateCommand : IRequest
{
    public Guid BoardId { get; set; }
    public Guid CardId { get; set; }
    public DateTime? DueDate { get; set; }
}
