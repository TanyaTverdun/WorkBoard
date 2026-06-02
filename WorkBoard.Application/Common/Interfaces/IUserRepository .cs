using System.Data;
using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces;

public interface IUserRepository : IGenericRepository<User, Guid>
{
}
