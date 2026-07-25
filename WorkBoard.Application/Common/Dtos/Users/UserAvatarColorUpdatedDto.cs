namespace WorkBoard.Application.Common.Dtos.Users;

public record UserAvatarColorUpdatedDto
{
    public Guid UserId { get; init; }
    public required string AvatarColor { get; init; }
}
