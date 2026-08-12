using WorkBoard.Domain.Enums;

namespace WorkBoard.Application.Common.Dtos.Workspaces;

public class UpdateWorkspaceMemberRoleRequest
{
    public WorkspaceRole NewRole { get; set; }
}
