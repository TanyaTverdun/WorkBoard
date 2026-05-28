namespace WorkBoard.Application.Common.Dtos.User;

/// <summary>
/// Data transfer object containing the profile changes sent from the UI
/// </summary>
public record UpdateUserProfileDto(
    string? FullName,
    string? AvatarUrl);
