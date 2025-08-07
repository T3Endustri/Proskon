namespace _02_Application.Dtos;
 
public abstract record BaseFormDto
{
    public Guid Id { get; init; }
    public Guid TemplateId { get; init; }
    public Guid CreateUserId { get; init; }
    public Guid? ApprovedUserId { get; init; }
    public bool IsApprove { get; init; }
    public DateTime CreateTime { get; init; }
}
public record FormDto : BaseFormDto
{
    public string CreateUserName { get; init; } = string.Empty;
    public string? ApprovedUserName { get; init; }

    public List<FormFieldDto> ListFormFields { get; init; } = [];
}
public record FormListDto : BaseFormDto
{
    public string CreateUserName { get; init; } = string.Empty;
    public string? ApprovedUserName { get; init; }
    public int FieldCount { get; init; }
}
public record FormSelectDto
{
    public Guid Id { get; init; }
    public string Display { get; init; } = string.Empty;
}
public record FormFieldDto
{
    public Guid Id { get; init; }
    public Guid PropertyFieldId { get; init; }
    public string PropertyName { get; init; } = string.Empty;

    public List<FormFieldValueDto> ListValues { get; init; } = [];
}
public record FormFieldValueDto
{
    public Guid Id { get; init; }
    public string Value { get; init; } = string.Empty;
    public DateTime CreateTime { get; init; }
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
}

public record FormResourceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public short DataType { get; init; }
    public bool IsSystemDefined { get; init; }
    public bool AllowMultipleSelection { get; init; }

    public List<FormResourceItemDto> ListItems { get; init; } = [];
}
public record FormResourceItemDto
{
    public Guid Id { get; init; }
    public string DisplayText { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public int SortBy { get; init; }
    public bool IsActive { get; init; }
}
public class FormResourceListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public Guid TemplateId { get; set; }
    public string? Description { get; set; }
}

public abstract record BasePropertyDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string DisplayText { get; init; } = string.Empty;
    public short FieldType { get; init; }
    public bool IsRequired { get; init; }
}

public record PropertyDto : BasePropertyDto
{
    public string Pattern { get; init; } = string.Empty;
    public string ExtField { get; init; } = string.Empty;
    public string FileTypes { get; init; } = string.Empty;
    public int MaxLength { get; init; }
    public bool FileMultiple { get; init; }
    public bool Range { get; init; }
    public double RMax { get; init; }
    public double RMin { get; init; }
    public double RStep { get; init; }

    public Guid? FormResourceId { get; init; }
    public string? FormResourceName { get; init; }

    public List<PropertyPanelDto> ListPanels { get; init; } = [];
    public List<PropertyTemplateDto> ListTemplates { get; init; } = [];
}

public record PropertyListDto : BasePropertyDto
{
    public string Pattern { get; init; } = string.Empty;
    public string FileTypes { get; init; } = string.Empty;
}
public record PropertySelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record PropertyPanelDto
{
    public Guid PropertyFieldId { get; init; }
    public Guid PanelId { get; init; }
    public int Column { get; init; }
    public int SortBy { get; init; }

    public string PanelName { get; init; } = string.Empty;
}
public record PropertyTemplateDto
{
    public Guid PropertyFieldId { get; init; }
    public Guid TemplateId { get; init; }
    public int Column { get; init; }
    public int SortBy { get; init; }
    public string PropertyName { get; init; } = string.Empty;
}
public abstract record BaseTemplateDto
{
    public Guid Id { get; init; }
    public int ColumnCount { get; init; }
}
public record TemplateDto : BaseTemplateDto
{
    public List<TemplatePanelDto> ListPanels { get; init; } = [];
    public List<TemplateApproverDto> ListApprovers { get; init; } = [];
    public List<PropertyTemplateDto> ListProperties { get; init; } = [];
}
public record TemplateListDto : BaseTemplateDto
{
    public int FormCount { get; init; }
    public int PanelCount { get; init; }
    public int ApproverCount { get; init; }
}
public record TemplatePanelDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int SortBy { get; init; }
}

public record TemplateApproverDto
{
    public Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public Guid? RoleId { get; init; }

    public string? UserName { get; init; }
    public string? RoleName { get; init; }
}