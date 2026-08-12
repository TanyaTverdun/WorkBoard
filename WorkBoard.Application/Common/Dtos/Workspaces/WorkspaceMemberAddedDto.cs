using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.Workspaces;

public record WorkspaceMemberAddedDto(
    Guid UserId,
    string Name,
    string Email,
    WorkspaceRole Role,
    string? AvatarUrl,
    string? AvatarColor,
    string? Initials
);
