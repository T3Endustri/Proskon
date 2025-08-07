using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class LocationService(
    IGenericService<T3Location> locationService,
    IGenericService<T3LocationHierarchy> hierarchyService,
    IMapper mapper
) : ILocationService
{
    public async Task<List<LocationListDto>> GetAllAsync()
    {
        var locations = await locationService.ListAsync(LocationSpec.All());
        return mapper.Map<List<LocationListDto>>(locations);
    }

    public async Task<LocationDto?> GetByIdAsync(Guid id)
    {
        var location = await locationService.ListAsync(LocationSpec.ById(id));
        return mapper.Map<LocationDto>(location.FirstOrDefault());
    }

    public async Task<List<LocationTreeDto>> GetTreeAsync()
    {
        var allLocations = await locationService.ListAsync(LocationSpec.Tree());
        var dict = allLocations.ToDictionary(l => l.Id);
        var tree = new List<LocationTreeDto>();

        foreach (var loc in allLocations.Where(l => l.ListParents.Count == 0))
            tree.Add(BuildTree(loc, 0));

        LocationTreeDto BuildTree(T3Location location, int level)
        {
            var dto = mapper.Map<LocationTreeDto>(location);
            dto.Level = level;
            dto.Children = [.. location.ListChilds
                .Select(c => dict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))];
            return dto;
        }

        return tree;
    }

    public async Task<List<LocationHierarchyDto>> GetFlatHierarchyAsync(Guid locationId)
    {
        var location = await locationService.GetByIdAsync(locationId, l => l.ListParents, l => l.ListChilds);
        if (location is null) return [];

        var hierarchies = location.ListParents.Concat(location.ListChilds).ToList();
        return mapper.Map<List<LocationHierarchyDto>>(hierarchies);
    }

    public async Task AddAsync(LocationDto dto)
    {
        var entity = mapper.Map<T3Location>(dto);
        await locationService.AddAsync(entity);
    }

    public async Task UpdateAsync(LocationDto dto)
    {
        var entity = mapper.Map<T3Location>(dto);
        await locationService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await locationService.DeleteAsync(id);
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var exists = await hierarchyService.AnyAsync(h =>
            h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3LocationHierarchy
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
