namespace WorkBoard.Application.Common.Dtos.Cards;

public record CardAssigneeDto(
    Guid UserId,
    string FullName,
    string Email,
    string? AvatarUrl,
    string Initials
);
