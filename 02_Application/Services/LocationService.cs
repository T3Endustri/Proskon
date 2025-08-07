using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class LocationService(IUnitOfWork unitOfWork, IMapper mapper) : ILocationService
{
    public async Task<List<LocationListDto>> GetAllAsync()
    {
        var locations = await unitOfWork.Repository<T3Location>().ListAsync(LocationSpec.All());
        return mapper.Map<List<LocationListDto>>(locations);
    }

    public async Task<LocationDto?> GetByIdAsync(Guid id)
    {
        var locations = await unitOfWork.Repository<T3Location>().ListAsync(LocationSpec.ById(id));
        return locations.Count == 0 ? null : mapper.Map<LocationDto>(locations[0]);
    }

    public async Task<List<LocationTreeDto>> GetTreeAsync()
    {
        var allLocations = await unitOfWork.Repository<T3Location>().ListAsync(LocationSpec.Tree());
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
        var location = await unitOfWork.Repository<T3Location>().GetByIdAsync(locationId, l => l.ListParents, l => l.ListChilds);
        if (location is null) return [];

        var hierarchies = location.ListParents.Concat(location.ListChilds).ToList();
        return mapper.Map<List<LocationHierarchyDto>>(hierarchies);
    }

    public async Task AddAsync(LocationDto dto)
    {
        var entity = mapper.Map<T3Location>(dto);
        await unitOfWork.Repository<T3Location>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(LocationDto dto)
    {
        var entity = mapper.Map<T3Location>(dto);
        await unitOfWork.Repository<T3Location>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Location>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3LocationHierarchy>();
        var exists = await repo.AnyAsync(h => h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3LocationHierarchy
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
        var repo = unitOfWork.Repository<T3LocationHierarchy>();
        var all = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var item in all)
            await repo.DeleteAsync(item.Id);

        await unitOfWork.SaveChangesAsync();
    }
}
