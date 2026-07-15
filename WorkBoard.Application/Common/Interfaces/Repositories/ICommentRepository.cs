using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces.Repositories;

public interface ICommentRepository : IGenericRepository<Comment, Guid>
{
}
