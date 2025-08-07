using _02_Application.Dtos;

namespace _02_Application.Interfaces;

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
