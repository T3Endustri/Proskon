using _01_Data.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

public class T3IdentityClaim : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public short PermissionType { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public T3IdentityRole? Role { get; set; } = default!;
    public T3IdentityUser? User { get; set; } = default!;

}
public class T3IdentityRole : BaseEntity
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string StartPage { get; set; } = string.Empty;
    [Required] public bool IsActive { get; set; } = true;
    [Required] public bool IsTeam { get; set; } = false;
    [Required] public bool IsDepartment { get; set; } = false;
    public ICollection<T3IdentityRoleHierarchy> ListParents { get; set; } = [];
    public ICollection<T3IdentityRoleHierarchy> ListChilds { get; set; } = [];
    public ICollection<T3IdentityUserRole> ListUsers { get; set; } = [];
    public ICollection<T3IdentityClaim> ListClaims { get; set; } = [];
    public ICollection<T3TemplateApprover> ListApproveTemplates { get; set; } = [];

}
public class T3IdentityRoleHierarchy : BaseEntity
{
    [Required] public Guid ParentId { get; set; }
    [Required] public Guid ChildId { get; set; }
    public T3IdentityRole Parent { get; set; } = default!;
    public T3IdentityRole Child { get; set; } = default!;
}
public class T3IdentityUser : BaseEntity
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
    [Required] public string Barcode { get; set; } = string.Empty;
    [Required] public string StartPage { get; set; } = string.Empty;
    [Required] public bool IsActive { get; set; } = true;
    [Required] public string PhotoUrl { get; set; } = string.Empty;
    public ICollection<T3Form> ListFormCreates { get; set; } = [];
    public ICollection<T3Form> ListFormApproveds { get; set; } = [];
    public ICollection<T3TemplateApprover> ListApproveTemplates { get; set; } = [];
    public ICollection<T3FormFieldValue> ListFormFieldValue { get; set; } = [];
    public ICollection<T3IdentityUserRole> ListRoles { get; set; } = [];
    public ICollection<T3IdentityClaim> ListClaims { get; set; } = [];
}
public class T3IdentityUserRole : BaseEntity
{
    [Required] public Guid UserId { get; set; } = default!;
    [Required] public Guid RoleId { get; set; } = default!;
    public T3IdentityUser User { get; set; } = default!;
    public T3IdentityRole Role { get; set; } = default!;
}

public class T3Form : BaseEntity
{
    public Guid TemplateId { get; set; }
    public Guid CreateUserId { get; set; }
    public Guid? ApprovedUserId { get; set; }
    public bool IsApprove { get; set; } = false;
    public DateTime CreateTime { get; set; } = DateTime.Now;
    [ForeignKey(nameof(Id))] public T3Item? Item { get; set; }
    [ForeignKey(nameof(Id))] public T3Module? Module { get; set; }
    public T3Template Template { get; set; } = default!;
    public T3IdentityUser CreateUser { get; set; } = default!;
    public T3IdentityUser? ApprovedUser { get; set; }
    public List<T3FormField> ListFormFields { get; set; } = [];
}
public class T3FormField : BaseEntity
{
    public Guid FormId { get; set; }
    public Guid PropertyFieldId { get; set; }
    public T3Form Form { get; set; } = default!;
    public T3Property PropertyField { get; set; } = default!;
    public List<T3FormFieldValue> ListValues { get; set; } = [];
}
public class T3FormFieldValue : BaseEntity
{
    public Guid FormFieldId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.Now;
    public string Value { get; set; } = string.Empty;
    public T3IdentityUser CreateUser { get; set; } = default!;
    public T3FormField FormField { get; set; } = default!;
}
public class T3FormResource : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public short DataType { get; set; } = 0;
    public bool IsSystemDefined { get; set; } = false;
    public bool AllowMultipleSelection { get; set; } = false;
    public ICollection<T3FormResourceItem> ListItems { get; set; } = [];
    public ICollection<T3Property> ListProperties { get; set; } = [];
}
public class T3FormResourceItem : BaseEntity
{
    public Guid ResourceId { get; set; }
    public string DisplayText { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int SortBy { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public T3FormResource T3DataResource { get; set; } = default!;
}

public class T3Property : BaseEntity
{
    public Guid? FormResourceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
    public string ExtField { get; set; } = string.Empty;
    public short FieldType { get; set; }
    public bool Range { get; set; } = false;
    public double RMax { get; set; } = double.MaxValue;
    public double RMin { get; set; } = double.MinValue;
    public double RStep { get; set; } = 1;
    public bool Require { get; set; } = false;
    public string Pattern { get; set; } = string.Empty;
    public int MaxLength { get; set; } = 100;
    public string FileTypes { get; set; } = string.Empty;
    public bool FileMultiple { get; set; } = false;
    public bool IsRequired { get; set; } = false;
    public T3FormResource? FormResource { get; set; }
    public ICollection<T3FormField> ListFormFields { get; set; } = [];
    public ICollection<T3PropertyTemplate> ListTemplates { get; set; } = [];
    public ICollection<T3PropertyPanel> ListPanels { get; set; } = [];
}
public class T3PropertyPanel : BaseEntity
{
    public Guid PropertyFieldId { get; set; }
    public Guid PanelId { get; set; }
    public int Column { get; set; } = 1;
    public int SortBy { get; set; } = 100;
    public T3Property PropertyField { get; set; } = default!;
    public T3TemplatePanel Panel { get; set; } = default!;
}
public class T3PropertyTemplate : BaseEntity
{
    public Guid PropertyFieldId { get; set; }
    public Guid TemplateId { get; set; }
    public int Column { get; set; } = 1;
    public int SortBy { get; set; } = 1;
    public T3Property PropertyField { get; set; } = default!;
    public T3Template Template { get; set; } = default!;
}
public class T3Template : BaseEntity
{
    public int ColumnCount { get; set; } = 1;
    public T3Module? Module { get; set; }
    public T3Item? Item { get; set; }

    public List<T3Form> ListForms { get; set; } = [];
    public List<T3TemplatePanel> ListPanels { get; set; } = [];
    public List<T3TemplateApprover> ListApprovers { get; set; } = [];
    public List<T3PropertyTemplate> ListPropertyFields { get; set; } = [];
}
public class T3TemplateApprover : BaseEntity
{
    public Guid TemplateId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public T3Template Template { get; set; } = default!;
    public T3IdentityUser? User { get; set; }
    public T3IdentityRole? Role { get; set; }
}
public class T3TemplatePanel : BaseEntity
{
    public Guid TemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SortBy { get; set; }
    public T3Template Template { get; set; } = default!;
    public ICollection<T3PropertyPanel> ListPropertyFields { get; set; } = [];
}
public partial class T3Shift() : BaseEntity
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public DateTime Finish { get; set; }
    public int Target { get; set; } = 0;
    public T3Location Location { get; set; } = default!;
    public List<T3ShiftBreak> ListBreaks { get; set; } = [];
}
public partial class T3ShiftBreak() : BaseEntity
{
    public Guid ShiftId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public T3Shift Shift { get; set; } = default!;
}
public partial class T3ShiftType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<T3ShiftTypeDay> ListDays { get; set; } = [];
    public ICollection<T3ShiftTypeLocation> ListLocations { get; set; } = [];
}
public partial class T3ShiftTypeBreak : BaseEntity
{
    public Guid ShiftTypeDayId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public T3ShiftTypeDay ShiftTypeDay { get; set; } = null!;
}
public partial class T3ShiftTypeCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
public partial class T3ShiftTypeDay : BaseEntity
{
    public Guid ShiftTypeId { get; set; }
    public byte DayOfWeek { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public T3ShiftType ShiftType { get; set; } = null!;
    public ICollection<T3ShiftTypeBreak> T3ShiftTypeBreaks { get; set; } = [];
}
public partial class T3ShiftTypeLocation : BaseEntity
{
    public Guid ShiftTypeId { get; set; }
    public Guid LocationId { get; set; }
    public int Value { get; set; }
    public DateTime Time { get; set; } = DateTime.Now;
    public T3Location Location { get; set; } = null!;
    public T3ShiftType ShiftType { get; set; } = null!;

}