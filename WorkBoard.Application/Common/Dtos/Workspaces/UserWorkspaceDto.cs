using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.Workspaces;

public record UserWorkspaceDto(
    Guid Id,
    string Name,
    SubscriptionTier SubscriptionTier,
    UserRole UserRole
);
