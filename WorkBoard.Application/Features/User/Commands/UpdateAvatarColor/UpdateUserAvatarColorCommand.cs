using MediatR;

namespace WorkBoard.Application.Features.User.Commands.UpdateAvatarColor;

public record UpdateUserAvatarColorCommand : IRequest
{
    public required string AvatarColor { get; init; }
}
