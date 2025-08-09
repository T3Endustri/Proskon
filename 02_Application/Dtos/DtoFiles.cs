using System.ComponentModel.DataAnnotations;

namespace _02_Application.Dtos;

#region Form Dtos
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
#endregion

#region Identity Dtos 
public class LoginDto
{
    [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [MinLength(4, ErrorMessage = "Şifre en az 4 karakter olmalı")]
    public string Password { get; set; } = string.Empty;
}

public record LoginResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public UserListDto? User { get; init; }
}
public abstract record BaseUserDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string PhotoUrl { get; init; } = string.Empty;
    public string StartPage { get; set; } = string.Empty;
}
public record UserListDto : BaseUserDto
{
    public List<RoleDto> Roles { get; init; } = [];
    public List<ClaimDto> Claims { get; init; } = [];
}
public record UserDto : BaseUserDto
{
    public string Password { get; init; } = string.Empty;
}
public record UserChangePasswordDto
{
    public Guid Id { get; init; }
    public string CurrentPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}
public abstract record BaseRoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string StartPage { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsTeam { get; init; }
    public bool IsDepartment { get; init; }
}
public record RoleDto : BaseRoleDto;
public record RoleListDto : BaseRoleDto
{
    public int ParentCount { get; init; }
    public int ChildCount { get; init; }
    public int UserCount { get; init; }
}
public record RoleTreeDto : BaseRoleDto
{
    public int Level { get; set; }
    public List<RoleTreeDto> Children { get; set; } = [];
}
public abstract record BaseClaimDto
{
    public string Type { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public short PermissionType { get; init; }
}
public record ClaimDto : BaseClaimDto
{
    public Guid Id { get; init; }
}
public record ClaimCreateDto : BaseClaimDto
{
    public Guid? UserId { get; init; }
    public Guid? RoleId { get; init; }
}
public record ClaimAssignDto
{
    public Guid? UserId { get; init; }
    public Guid? RoleId { get; init; }
    public List<ClaimCreateDto> Claims { get; init; } = [];
}
public record ClaimGroupDto
{
    public string GroupName { get; init; } = string.Empty;
    public List<ClaimDto> Claims { get; init; } = [];
}
#endregion

#region Location Dtos
public abstract record BaseLocationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public int SortBy { get; init; }
    public int OperationNo { get; init; }
    public bool IsStation { get; init; }
    public List<LocationHierarchyDto> ListParents { get; init; } = [];
    public List<LocationHierarchyDto> ListChilds { get; init; } = [];
}
public record LocationDto : BaseLocationDto;
public record LocationListDto : BaseLocationDto
{
    public int ParentCount { get; init; }
    public int ChildCount { get; init; }
    public int ItemCount { get; init; }
}
public record LocationTreeDto : BaseLocationDto
{
    public int Level { get; set; }
    public List<LocationTreeDto> Children { get; set; } = [];
}
public record LocationHierarchyDto
{
    public Guid ParentId { get; init; }
    public string ParentName { get; init; } = string.Empty;

    public Guid ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
}
#endregion

#region Module Dtos
public abstract record BaseModuleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PageText { get; init; } = string.Empty;
    public bool IsCanPage { get; init; }
    public bool IsCanShift { get; init; }
    public bool IsCanFilter { get; init; }
    public bool IsCanTemplate { get; init; }
    public bool IsCanBarcode { get; init; }
    public bool IsCanModuleType { get; init; }
    public bool IsCanTarget { get; init; }
    public bool IsCanSerial { get; init; }
    public int IconMultiple { get; set; } = 0;
    public int IconSingle { get; set; } = 0;
    public string ColorBack { get; init; } = string.Empty;
    public string ColorFore { get; init; } = string.Empty;
    public int SortBy { get; init; }
    public List<ModuleHierarchyDto> ListParents { get; init; } = [];
    public List<ModuleHierarchyDto> ListChilds { get; init; } = [];
}
public record ModuleDto : BaseModuleDto;
public record ModuleListDto : BaseModuleDto
{
    public int ParentCount { get; init; }
    public int ChildCount { get; init; }
}
public record ModuleTreeDto : BaseModuleDto
{
    public int Level { get; set; }
    public List<ModuleTreeDto> Children { get; set; } = [];
}
public record ModuleHierarchyDto
{
    public Guid ParentId { get; init; }
    public string ParentName { get; init; } = string.Empty;

    public Guid ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
}
#endregion

#region Item Dtos
public abstract record BaseItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public string ExternalFilter { get; init; } = string.Empty;
    public string SerialNumber { get; init; } = string.Empty;
    public long Target { get; init; }
    public short Status { get; init; }
    public int SortBy { get; init; }
    public List<ItemHierarchyDto> ListParents { get; init; } = [];
    public List<ItemHierarchyDto> ListChilds { get; init; } = [];
}
public record ItemDto : BaseItemDto
{
    public Guid ModuleId { get; init; }
    public Guid? LocationId { get; init; }
    public Guid? ModuleTypeId { get; init; }
}
public record ItemListDto : BaseItemDto
{
    public string ModuleName { get; init; } = string.Empty;
    public string? LocationName { get; init; }
    public string? ModuleTypeName { get; init; }
}
public record ItemSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ItemTreeDto : BaseItemDto
{
    public int Level { get; set; }
    public List<ItemTreeDto> Children { get; set; } = [];
}
public record ItemHierarchyDto
{
    public Guid ParentId { get; init; }
    public string ParentName { get; init; } = string.Empty;

    public Guid ChildId { get; init; }
    public string ChildName { get; init; } = string.Empty;
}
#endregion

#region ProcessType Dtos
public abstract record BaseProcessTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Barcode { get; init; } = string.Empty;
    public int IconMultiple { get; init; }
    public int IconSingle { get; init; }
    public string ColorBack { get; init; } = string.Empty;
    public string ColorFore { get; init; } = string.Empty;
    public long Target { get; init; }
    public int SortBy { get; init; }
}
public record ProcessTypeDto : BaseProcessTypeDto
{
    public List<ProcessTypeModuleDto> ListModules { get; init; } = [];
    public List<ProcessTypeItemDto> ListItems { get; init; } = [];
}
public record ProcessTypeListDto : BaseProcessTypeDto
{
    public int ModuleCount { get; init; }
    public int ItemCount { get; init; }
    public int ProtocolCount { get; init; }
}
public record ProcessTypeSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ProcessTypeItemDto
{
    public Guid ItemId { get; init; }
    public string ItemName { get; init; } = string.Empty;
}
public record ProcessTypeModuleDto
{
    public Guid ModuleId { get; init; }
    public string ModuleName { get; init; } = string.Empty;
}
#endregion

#region ProtocolType Dtos
public abstract record BaseProtocolDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public long Target { get; init; }
    public int SortBy { get; init; }
}
public record ProtocolDto : BaseProtocolDto
{
    public Guid ProcessTypeId { get; init; }
    public string ProcessTypeName { get; init; } = string.Empty;

    public List<ProtocolItemDto> ListProtocolItems { get; init; } = [];
}
public record ProtocolListDto : BaseProtocolDto
{
    public string ProcessTypeName { get; init; } = string.Empty;
    public int ItemCount { get; init; }
}
public record ProtocolSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ProtocolItemDto
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
    public string ItemName { get; init; } = string.Empty;

    public Guid LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;

    public long Target { get; init; }
}
#endregion

#region Shift Dtos
public abstract record BaseShiftTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<ShiftTypeDayDto> ListDays { get; init; } = [];
    public List<ShiftTypeLocationDto> ListLocations { get; init; } = [];
}
public record ShiftTypeDto : BaseShiftTypeDto;
public record ShiftTypeListDto : BaseShiftTypeDto
{
    public int DayCount { get; init; }
    public int LocationCount { get; init; }
}
public record ShiftTypeSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ShiftTypeDayDto
{
    public Guid Id { get; init; }
    public byte DayOfWeek { get; init; }
    public long StartTime { get; init; }
    public long EndTime { get; init; }

    public List<ShiftTypeBreakDto> ListBreaks { get; init; } = [];
}
public record ShiftTypeBreakDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public long StartTime { get; init; }
    public long EndTime { get; init; }
}
public record ShiftTypeLocationDto
{
    public Guid ShiftTypeId { get; init; }
    public string ShiftTypeName { get; init; } = string.Empty;

    public Guid LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;
}
public abstract record BaseShiftDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime Start { get; init; }
    public DateTime? End { get; init; }
    public DateTime Finish { get; init; }
    public int Target { get; init; }
}
public record ShiftDto : BaseShiftDto
{
    public Guid LocationId { get; init; }
    public string LocationName { get; init; } = string.Empty;
    public List<ShiftBreakDto> Breaks { get; init; } = [];
}
public record ShiftListDto : BaseShiftDto
{
    public string LocationName { get; init; } = string.Empty;
    public int BreakCount { get; init; }
}
public record ShiftSelectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
public record ShiftBreakDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
}
#endregion