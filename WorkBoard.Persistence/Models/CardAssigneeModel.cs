using WorkBoard.Application.Common.Models;

namespace WorkBoard.Persistence.Models;

internal class CardAssigneeModel :AssigneeModel
{
    public Guid CardId { get; set; }
}
