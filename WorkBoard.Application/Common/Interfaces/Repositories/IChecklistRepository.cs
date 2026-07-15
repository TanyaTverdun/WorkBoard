using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IChecklistRepository : IGenericRepository<Checklist, Guid>
{
}
