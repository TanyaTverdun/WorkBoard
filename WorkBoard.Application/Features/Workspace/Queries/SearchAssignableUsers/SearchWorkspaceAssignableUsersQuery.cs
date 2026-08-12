using MediatR;
using WorkBoard.Application.Common.Dtos.Users;

namespace WorkBoard.Application.Features.Workspace.Queries.SearchAssignableUsers;

public record SearchWorkspaceAssignableUsersQuery(
    Guid WorkspaceId,
    string SearchTerm) : IRequest<IReadOnlyList<UserSearchDto>>;
