using _01_Data.Entities.Base;   
using System.ComponentModel.DataAnnotations.Schema;

namespace _01_Data.Entities;

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