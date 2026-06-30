using System.ComponentModel.DataAnnotations.Schema;
using WorkBoard.Domain.Common;

namespace WorkBoard.Domain.Entities;

[Table("CardLabels")]
public class CardLabel : BaseEntity<(Guid, Guid)>
{
    public Guid CardId { get; set; }

    public Guid LabelId { get; set; }
}
