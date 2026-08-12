using WorkBoard.Domain.Entities;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Interfaces;

public interface IUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    string? FullName { get; }
    Guid? CurrentWorkspaceId {  get; }
    WorkspaceRole? CurrentWorkspaceRole { get; }
    Task SetWorkspaceContextAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default);
    Task<User?> GetCurrentUserFullProfileAsync(
        CancellationToken cancellationToken = default);
}
