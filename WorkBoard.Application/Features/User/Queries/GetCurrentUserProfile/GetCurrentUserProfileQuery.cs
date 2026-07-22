using MediatR;
using WorkBoard.Application.Common.Dtos.Users;

namespace WorkBoard.Application.Features.User.Queries.GetCurrentUserProfile;

public record GetCurrentUserProfileQuery : IRequest<UserProfileDto>;
