using MediatR;

namespace WorkBoard.Application.Features.User.Commands.UpdateAvatarImage;

public record UpdateUserAvatarImageCommand : IRequest
{
    public required Stream FileStream { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required long Length { get; init; }
}
