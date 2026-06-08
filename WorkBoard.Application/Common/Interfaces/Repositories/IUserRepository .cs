using System.Data;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<User, Guid>
{
}
