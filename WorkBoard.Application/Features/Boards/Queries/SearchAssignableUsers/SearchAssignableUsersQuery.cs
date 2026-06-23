using MediatR;
using WorkBoard.Application.Common.Dtos.Users;

namespace WorkBoard.Application.Features.Boards.Queries.SearchAssignableUsers;

public record SearchAssignableUsersQuery(
    Guid BoardId,
    string SearchTerm) : IRequest<IReadOnlyList<UserSearchDto>>;
