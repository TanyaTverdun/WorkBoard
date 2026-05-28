namespace WorkBoard.Application.Common.Dtos.User;

/// <summary>
/// A data transfer object representing user information
/// </summary>
public record UserDto(
    Guid UserId,
    string? FullName,
    string Email,
    string? AvatarUrl);
