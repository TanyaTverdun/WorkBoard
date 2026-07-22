using WorkBoard.Domain.Entities;

namespace WorkBoard.Application.Common.Interfaces;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
    Task<User?> GetCurrentUserFullProfileAsync(
        CancellationToken cancellationToken = default);
}
