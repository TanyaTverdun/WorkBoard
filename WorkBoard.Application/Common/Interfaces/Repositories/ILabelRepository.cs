using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ILabelRepository 
    : IGenericRepository<Label, Guid>
{
    Task<bool> IsNameUniqueAsync(
        Guid boardId,
        string name,
        CancellationToken cancellationToken = default);

}
