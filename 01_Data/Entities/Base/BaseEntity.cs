using _01_Data.Utilities;
using System.ComponentModel.DataAnnotations;

namespace _01_Data.Entities.Base;

public abstract class BaseEntity
{
    [Key]
    [Required]
    public Guid Id { get; set; } = SequentialGuid.NewGuid();

    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;
}
