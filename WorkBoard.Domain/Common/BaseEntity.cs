using System.ComponentModel.DataAnnotations;

namespace WorkBoard.Domain.Common;

public abstract class BaseEntity<TId>
{
    [Key]
    public virtual TId Id { get; set; } = default!;
}
