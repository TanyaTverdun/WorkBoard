using MediatR;

namespace WorkBoard.Application.Features.User.Commands.UpdateUserProfile;

/// <summary>
/// Command data structure dispatched from the API layer.
/// </summary>
public record UpdateUserProfileCommand(
    Guid UserId,
    string? FullName,
    string? AvatarUrl) : IRequest<bool>;
