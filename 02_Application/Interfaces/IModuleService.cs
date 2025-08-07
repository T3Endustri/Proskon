using _02_Application.Dtos;

namespace _02_Application.Interfaces;

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
