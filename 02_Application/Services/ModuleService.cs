using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ModuleService(
    IGenericService<T3Module> moduleService,
    IGenericService<T3ModuleHierarchy> hierarchyService,
    IMapper mapper
) : IModuleService
{
    public async Task<List<ModuleListDto>> GetAllAsync()
    {
        var modules = await moduleService.ListAsync(ModuleSpec.All());
        return mapper.Map<List<ModuleListDto>>(modules);
    }

    public async Task<ModuleDto?> GetByIdAsync(Guid id)
    {
        var module = await moduleService.ListAsync(ModuleSpec.ById(id));
        return mapper.Map<ModuleDto>(module.FirstOrDefault());
    }

    public async Task<List<ModuleTreeDto>> GetTreeAsync()
    {
        var allModules = await moduleService.ListAsync(ModuleSpec.Tree());
        var dict = allModules.ToDictionary(m => m.Id);
        var tree = new List<ModuleTreeDto>();

        foreach (var mod in allModules.Where(m => m.ListParents.Count == 0))
            tree.Add(BuildTree(mod, 0));

        ModuleTreeDto BuildTree(T3Module module, int level)
        {
            var dto = mapper.Map<ModuleTreeDto>(module);
            dto.Level = level;
            dto.Children = [.. module.ListChilds
                .Select(c => dict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))];
            return dto;
        }

        return tree;
    }

    public async Task<List<ModuleHierarchyDto>> GetFlatHierarchyAsync(Guid moduleId)
    {
        var module = await moduleService.GetByIdAsync(moduleId, m => m.ListParents, m => m.ListChilds);
        if (module is null) return [];

        var hierarchies = module.ListParents.Concat(module.ListChilds).ToList();
        return mapper.Map<List<ModuleHierarchyDto>>(hierarchies);
    }

    public async Task AddAsync(ModuleDto dto)
    {
        var entity = mapper.Map<T3Module>(dto);
        await moduleService.AddAsync(entity);
    }

    public async Task UpdateAsync(ModuleDto dto)
    {
        var entity = mapper.Map<T3Module>(dto);
        await moduleService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await moduleService.DeleteAsync(id);
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var exists = await hierarchyService.AnyAsync(h =>
            h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3ModuleHierarchy
            {
                Id = Guid.NewGuid(),
                ChildId = childId,
                ParentId = parentId
            };
            await hierarchyService.AddAsync(relation);
        }
    }

    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        var all = await hierarchyService.WhereAsync(h =>
            h.ChildId == childId && h.ParentId == parentId);
        foreach (var item in all)
            await hierarchyService.DeleteAsync(item.Id);
    }
}
