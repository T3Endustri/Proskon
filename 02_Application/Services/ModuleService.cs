using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ModuleService(IUnitOfWork unitOfWork, IMapper mapper) : IModuleService
{
    public async Task<List<ModuleListDto>> GetAllAsync()
    {
        var modules = await unitOfWork.Repository<T3Module>().ListAsync(ModuleSpec.All());
        return mapper.Map<List<ModuleListDto>>(modules);
    }

    public async Task<ModuleDto?> GetByIdAsync(Guid id)
    {
        var modules = await unitOfWork.Repository<T3Module>().ListAsync(ModuleSpec.ById(id));
        return modules.Count == 0 ? null : mapper.Map<ModuleDto>(modules[0]);
    }

    public async Task<List<ModuleTreeDto>> GetTreeAsync()
    {
        var allModules = await unitOfWork.Repository<T3Module>().ListAsync(ModuleSpec.Tree());
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
        var module = await unitOfWork.Repository<T3Module>().GetByIdAsync(moduleId, m => m.ListParents, m => m.ListChilds);
        if (module is null) return [];

        var hierarchies = module.ListParents.Concat(module.ListChilds).ToList();
        return mapper.Map<List<ModuleHierarchyDto>>(hierarchies);
    }

    public async Task AddAsync(ModuleDto dto)
    {
        var entity = mapper.Map<T3Module>(dto);
        await unitOfWork.Repository<T3Module>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(ModuleDto dto)
    {
        var entity = mapper.Map<T3Module>(dto);
        await unitOfWork.Repository<T3Module>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Module>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3ModuleHierarchy>();
        var exists = await repo.AnyAsync(h => h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3ModuleHierarchy
            {
                Id = Guid.NewGuid(),
                ChildId = childId,
                ParentId = parentId
            };
            await repo.AddAsync(relation);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3ModuleHierarchy>();
        var all = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var item in all)
            await repo.DeleteAsync(item.Id);

        await unitOfWork.SaveChangesAsync();
    }
}
