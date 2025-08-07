using _02_Application.Dtos;

namespace _02_Application.Interfaces;
 

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
