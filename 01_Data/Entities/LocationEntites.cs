using _01_Data.Entities.Base;  

namespace _01_Data.Entities;

public class T3Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int SortBy { get; set; } = 100;
    public int OperationNo { get; set; } = 0;
    public bool IsStation { get; set; }
    public List<T3LocationHierarchy> ListParents { get; set; } = [];
    public List<T3LocationHierarchy> ListChilds { get; set; } = [];
    public List<T3Item> ListItems { get; set; } = [];
    public List<T3LocationItem> ListLocationItems { get; set; } = [];
    public List<T3ShiftTypeLocation> ListShiftTypes { get; set; } = [];
    public List<T3Shift> ListShifts { get; set; } = [];
    public List<T3ProtocolItem> ListProtocolItems { get; set; } = [];

}
public class T3LocationHierarchy : BaseEntity
{
    public Guid ParentId { get; set; }
    public Guid ChildId { get; set; }
    public T3Location Parent { get; set; } = default!;
    public T3Location Child { get; set; } = default!;
}
public class T3LocationItem : BaseEntity
{
    public Guid ItemId { get; set; } = default!;
    public Guid LocationId { get; set; } = default!;
    public DateTime Entry { get; set; } = DateTime.Now;
    public DateTime? Exit { get; set; }
    public T3Item Item { get; set; } = default!;
    public T3Location Location { get; set; } = default!;
}
