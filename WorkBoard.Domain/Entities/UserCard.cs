using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("UserCards")]
public class UserCard : BaseEntity<(Guid, Guid)>
{
    public Guid UserId { get; set; }

    public Guid CardId { get; set; }
}
