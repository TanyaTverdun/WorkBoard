using WorkBoard.Application.Common.Dtos.Cards;
using WorkBoard.Application.Common.Dtos.Section;

namespace WorkBoard.Application.Common.Interfaces.Notification;

public interface IBoardNotificationService
{
    Task SendCardCreatedAsync(
        Guid boardId, 
        CardDto card, 
        CancellationToken cancellationToken = default);

    Task SendSectionCreatedAsync(
        Guid boardId, 
        SectionDto section, 
        CancellationToken cancellationToken = default);
}
