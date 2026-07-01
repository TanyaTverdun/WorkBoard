using WorkBoard.Application.Common.Dtos.Labels;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ILabelRepository 
    : IGenericRepository<Label, Guid>
{
    Task<bool> IsNameUniqueAsync(
        Guid boardId,
        string name,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Label>> GetByBoardIdAsync(
        Guid boardId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Label>> GetByCardIdAsync(
        Guid cardId,
        CancellationToken cancellationToken = default);
}
