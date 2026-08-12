using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.Workspaces;

public class AddWorkspaceMemberRequest
{
    public Guid UserId { get; set; }
    public WorkspaceRole Role { get; set; }
}
