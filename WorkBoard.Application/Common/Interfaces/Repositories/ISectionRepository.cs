using WorkBoard.Application.Common.Dtos.Section;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ISectionRepository : IGenericRepository<Section, Guid>
{
    Task<IReadOnlyList<SectionDto>> GetByBoardIdAsync(
        Guid boardId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
