using _01_Data.Entities;
using _02_Application.Dtos;
namespace _02_Application.Interfaces;

public interface ILogService
{
    void Info(string source, string message);
    void Warning(string source, string message);
    void Error(string source, string message, Exception? ex = null);
}

public interface IAppSeeder
{
    Task SeedAsync();
}


public interface IClaimService
{
    Task<List<ClaimDto>> GetAllAsync();
    Task<List<ClaimDto>> GetByUserIdAsync(Guid userId);
    Task<List<ClaimDto>> GetByRoleIdAsync(Guid roleId);
    Task AssignClaimsToUserAsync(ClaimAssignDto dto);
    Task AssignClaimsToRoleAsync(ClaimAssignDto dto);
    Task DeleteAsync(Guid claimId);
}

public interface IFormResourceService
{
    Task<List<FormResourceListDto>> GetAllAsync();
    Task<FormResourceDto?> GetByIdAsync(Guid id);
    Task AddAsync(FormResourceDto dto);
    Task UpdateAsync(FormResourceDto dto);
    Task DeleteAsync(Guid id);
}

public interface IFormService
{
    Task<List<FormListDto>> GetAllAsync();
    Task<FormDto?> GetByIdAsync(Guid id);
    Task<List<FormListDto>> GetByUserAsync(Guid userId);
    Task<List<FormListDto>> GetByTemplateAsync(Guid templateId);
    Task<List<FormListDto>> GetUnapprovedAsync();
    Task<(List<FormListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take);
    Task AddAsync(FormDto dto);
    Task UpdateAsync(FormDto dto);
    Task DeleteAsync(Guid id);
}

public interface IItemService
{
    Task<List<ItemListDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(Guid id);
    Task<List<ItemTreeDto>> GetTreeAsync();
    Task<List<ItemHierarchyDto>> GetFlatHierarchyAsync(Guid itemId);
    Task AddAsync(ItemDto dto);
    Task UpdateAsync(ItemDto dto);
    Task DeleteAsync(Guid id);
    Task AssignParentAsync(Guid childId, Guid parentId);
    Task RemoveParentAsync(Guid childId, Guid parentId);
}

public interface ILocationService
{
    Task<List<LocationListDto>> GetAllAsync();
    Task<LocationDto?> GetByIdAsync(Guid id);
    Task<List<LocationTreeDto>> GetTreeAsync();
    Task<List<LocationHierarchyDto>> GetFlatHierarchyAsync(Guid locationId);
    Task AddAsync(LocationDto dto);
    Task UpdateAsync(LocationDto dto);
    Task DeleteAsync(Guid id);
    Task AssignParentAsync(Guid childId, Guid parentId);
    Task RemoveParentAsync(Guid childId, Guid parentId);
}

public interface IModuleService
{
    Task<List<ModuleListDto>> GetAllAsync();
    Task<ModuleDto?> GetByIdAsync(Guid id);
    Task<List<ModuleTreeDto>> GetTreeAsync();
    Task<List<ModuleHierarchyDto>> GetFlatHierarchyAsync(Guid moduleId);
    Task AddAsync(ModuleDto dto);
    Task UpdateAsync(ModuleDto dto);
    Task DeleteAsync(Guid id);
    Task AssignParentAsync(Guid childId, Guid parentId);
    Task RemoveParentAsync(Guid childId, Guid parentId);
}

public interface IProcessTypeService
{
    Task<List<ProcessTypeListDto>> GetAllAsync();
    Task<ProcessTypeDto?> GetByIdAsync(Guid id);
    Task<List<ProcessTypeListDto>> SearchAsync(string keyword);
    Task<(List<ProcessTypeListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take);
    Task AddAsync(ProcessTypeDto dto);
    Task UpdateAsync(ProcessTypeDto dto);
    Task DeleteAsync(Guid id);
    Task AssignItemAsync(Guid typeId, Guid itemId);
    Task AssignModuleAsync(Guid typeId, Guid moduleId);
    Task RemoveItemAsync(Guid typeId, Guid itemId);
    Task RemoveModuleAsync(Guid typeId, Guid moduleId);
}

public interface IPropertyService
{
    Task<List<PropertyListDto>> GetAllAsync();
    Task<PropertyDto?> GetByIdAsync(Guid id);
    Task<List<PropertyListDto>> SearchAsync(string keyword);
    Task<List<PropertyListDto>> GetByFormResourceAsync(Guid resourceId);
    Task<List<PropertyDto>> GetRequiredAsync();
    Task AddAsync(PropertyDto dto);
    Task UpdateAsync(PropertyDto dto);
    Task DeleteAsync(Guid id);
}

public interface IProtocolService
{
    Task<List<ProtocolListDto>> GetAllAsync();
    Task<ProtocolDto?> GetByIdAsync(Guid id);
    Task<List<ProtocolListDto>> GetByProcessTypeAsync(Guid processTypeId);
    Task AddAsync(ProtocolDto dto);
    Task UpdateAsync(ProtocolDto dto);
    Task DeleteAsync(Guid id);
}

public interface IRoleService
{
    Task<List<RoleListDto>> GetAllAsync();
    Task<RoleDto?> GetByIdAsync(Guid id);
    Task<List<RoleListDto>> SearchAsync(string keyword);
    Task<(List<RoleListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take);
    Task<List<RoleDto>> GetByUserIdAsync(Guid userId);
    Task<List<RoleTreeDto>> GetTreeAsync();
    Task AddAsync(RoleDto dto);
    Task UpdateAsync(RoleDto dto);
    Task DeleteAsync(Guid id);
    Task<List<T3IdentityRole>> GetAllChildRolesRecursiveAsync(Guid roleId);
    Task<List<T3IdentityRole>> GetAllParentRolesRecursiveAsync(Guid roleId);
}

public interface IShiftService
{
    Task<List<ShiftListDto>> GetAllAsync();
    Task<ShiftDto?> GetByIdAsync(Guid id);
    Task<List<ShiftListDto>> GetByLocationAsync(Guid locationId);
    Task AddAsync(ShiftDto dto);
    Task UpdateAsync(ShiftDto dto);
    Task DeleteAsync(Guid id);
}

public interface IShiftTypeService
{
    Task<List<ShiftTypeListDto>> GetAllAsync();
    Task<ShiftTypeDto?> GetByIdAsync(Guid id);
    Task AddAsync(ShiftTypeDto dto);
    Task UpdateAsync(ShiftTypeDto dto);
    Task DeleteAsync(Guid id);
}

public interface ITemplateService
{
    Task<List<TemplateListDto>> GetAllAsync();
    Task<TemplateDto?> GetByIdAsync(Guid id);
    Task<List<TemplateListDto>> GetByApproverAsync(Guid userId);
    Task AddAsync(TemplateDto dto);
    Task UpdateAsync(TemplateDto dto);
    Task DeleteAsync(Guid id);
}

public interface IUserService
{
    Task<UserListDto?> GetByIdAsync(Guid id);
    Task<List<UserListDto>> GetAllAsync();
    Task AddAsync(UserDto dto);
    Task UpdateAsync(UserDto dto);
    Task ChangePasswordAsync(UserChangePasswordDto dto);
    Task DeleteAsync(Guid id);
}

public interface IAuthService
{
    Task<LoginResultDto> LoginAsync(LoginDto dto);
}
