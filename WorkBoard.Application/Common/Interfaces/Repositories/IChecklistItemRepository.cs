using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IChecklistItemRepository 
    : IGenericRepository<ChecklistItem, Guid>
{
}
