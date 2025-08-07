using _01_Data.Entities.Base; 
using System.ComponentModel.DataAnnotations.Schema;

namespace _01_Data.Entities;

public class T3Item : BaseEntity
{
    public Guid ModuleId { get; set; }
    public Guid? LocationId { get; set; }
    public Guid? ModuleTypeId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string ExternalFilter { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public long Target { get; set; } = 0;
    public short Status { get; set; } = 0;
    public int SortBy { get; set; } = 100;

    [ForeignKey(nameof(Id))] public T3Template? Template { get; set; }
    [ForeignKey(nameof(Id))] public T3Form? Form { get; set; }

    [ForeignKey(nameof(LocationId))] public T3Location? Location { get; set; }
    [ForeignKey(nameof(ModuleId))] public T3Module Module { get; set; } = default!;
    [ForeignKey(nameof(ModuleTypeId))] public T3Module ModuleType { get; set; } = default!;

    public ICollection<T3ProtocolItem> ListProtocols { get; set; } = [];
    public ICollection<T3ItemHierarchy> ListParents { get; set; } = [];
    public ICollection<T3ItemHierarchy> ListChilds { get; set; } = [];
    public ICollection<T3LocationItem> ListLocations { get; set; } = [];
    public ICollection<T3ProcessTypeItem> ListProcessTypes { get; set; } = [];
}

public class T3ItemHierarchy : BaseEntity
{
    public Guid ParentId { get; set; }
    public Guid ChildId { get; set; }
    public T3Item Parent { get; set; } = default!;
    public T3Item Child { get; set; } = default!;
}
public class T3Module : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string PageText { get; set; } = string.Empty;
    public int IconMultiple { get; set; } = 0;
    public int IconSingle { get; set; } = 0;
    public bool IsCanPage { get; set; } = false;
    public bool IsCanShift { get; set; } = false;
    public bool IsCanFilter { get; set; } = false;
    public bool IsCanTemplate { get; set; } = false;
    public bool IsCanBarcode { get; set; } = false;
    public bool IsCanModuleType { get; set; } = false;
    public bool IsCanTarget { get; set; } = false;
    public bool IsCanSerial { get; set; } = false;
    public string ColorBack { get; set; } = string.Empty;
    public string ColorFore { get; set; } = string.Empty;
    public int SortBy { get; set; } = 100;

    [ForeignKey(nameof(Id))] public T3Template? Template { get; set; }
    [ForeignKey(nameof(Id))] public T3Form? Form { get; set; }

    public ICollection<T3ModuleHierarchy> ListParents { get; set; } = [];
    public ICollection<T3ModuleHierarchy> ListChilds { get; set; } = [];
    public ICollection<T3Item> ListItems { get; set; } = [];
    public ICollection<T3Item> ListModuleTypeItems { get; set; } = [];
    public ICollection<T3ProcessTypeModule> ListProcessTypes { get; set; } = [];
}
public class T3ModuleHierarchy : BaseEntity
{
    public Guid ParentId { get; set; }
    public Guid ChildId { get; set; }
    public T3Module Parent { get; set; } = default!;
    public T3Module Child { get; set; } = default!;
}
public class T3ProcessType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int IconMultiple { get; set; } = 0;
    public int IconSingle { get; set; } = 0;
    public string ColorBack { get; set; } = string.Empty;
    public string ColorFore { get; set; } = string.Empty;
    public long Target { get; set; } = 0;
    public int SortBy { get; set; } = 100;
    public List<T3ProcessTypeModule> ListModules { get; set; } = [];
    public List<T3ProcessTypeItem> ListItems { get; set; } = [];
    public List<T3Protocol> ListProtocols { get; set; } = [];
}
public class T3ProcessTypeItem : BaseEntity
{
    public Guid TypeId { get; set; }
    public Guid ItemId { get; set; }
    public T3ProcessType ProcessType { get; set; } = default!;
    public T3Item Item { get; set; } = default!;
}
public class T3ProcessTypeModule : BaseEntity
{
    public Guid TypeId { get; set; }
    public Guid ModuleId { get; set; }
    public T3ProcessType ProcessType { get; set; } = default!;
    public T3Module Module { get; set; } = default!;
} 