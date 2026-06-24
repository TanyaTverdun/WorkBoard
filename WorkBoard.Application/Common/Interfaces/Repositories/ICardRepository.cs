using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ICardRepository : IGenericRepository<Card, Guid>
{
}
