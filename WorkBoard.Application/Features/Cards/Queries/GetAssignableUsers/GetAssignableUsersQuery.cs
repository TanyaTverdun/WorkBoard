using MediatR;
using WorkBoard.Application.Common.Dtos.Users;

namespace WorkBoard.Application.Features.Cards.Queries.GetAssignableUsers;

public record GetAssignableUsersQuery(Guid CardId) 
    : IRequest<IReadOnlyList<UserSearchDto>>;
