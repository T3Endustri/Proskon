using _01_Data.Entities.Base;  
namespace _01_Data.Entities;

public class T3Protocol : BaseEntity
{
    public Guid ProcessTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long Target { get; set; } = 0;
    public int SortBy { get; set; } = 100;
    public T3ProcessType ProcessType { get; set; } = default!;
    public List<T3ProtocolItem> ListProtocolItems { get; set; } = [];
}
public class T3ProtocolItem : BaseEntity
{
    public Guid ProtocolId { get; set; }
    public Guid ItemId { get; set; }
    public Guid LocationId { get; set; }
    public long Target { get; set; } = 0;
    public T3Protocol Protocol { get; set; } = default!;
    public T3Item Item { get; set; } = default!;
    public T3Location Location { get; set; } = default!;
}