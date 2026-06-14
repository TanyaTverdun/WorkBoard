using WorkBoard.Domain.Common;
using WorkBoard.Domain.Enums;

namespace WorkBoard.Domain.Entities;

public class BoardMember : BaseEntity<(Guid, Guid)>
{
    public Guid UserId { get; set; }

    public Guid BoardId { get; set; }

    public BoardRole UserRole { get; set; }
}
