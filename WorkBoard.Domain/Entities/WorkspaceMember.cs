using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Domain.Entities;

[Table("WorkspaceMembers")]
public class WorkspaceMember
{
    public Guid UserId { get; set; }

    public Guid WorkspaceId { get; set; }

    public WorkspaceRole UserRole { get; set; } = 
        WorkspaceRole.Member;
}
