using WorkBoard.Application.Common.Models;

namespace WorkBoard.Domain.Models;

internal class CardLabelModel : LabelModel
{
    public Guid CardId { get; set; }
}
