using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Domain.Entities;

[Table("BoardMembers")]
public class BoardMember : BaseEntity<(Guid, Guid)>
{
    public Guid UserId { get; set; }

    public Guid BoardId { get; set; }

    public UserRole UserRole { get; set; } = UserRole.Member;
}
