using MediatR;

namespace WorkBoard.Application.Features.Cards.Commands.DeleteCard;

public class DeleteCardCommand : IRequest<Unit>
{
    public Guid BoardId { get; set; }
    public Guid CardId { get; set; }
}
