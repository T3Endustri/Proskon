using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ItemService(IUnitOfWork unitOfWork, IMapper mapper) : IItemService
{
    public async Task<List<ItemListDto>> GetAllAsync()
    {
        var items = await unitOfWork.Repository<T3Item>().ListAsync(ItemSpec.All());
        return mapper.Map<List<ItemListDto>>(items);
    }

    public async Task<ItemDto?> GetByIdAsync(Guid id)
    {
        var items = await unitOfWork.Repository<T3Item>().ListAsync(ItemSpec.ById(id));
        return items.Count == 0 ? null : mapper.Map<ItemDto>(items[0]);
    }

    public async Task<List<ItemTreeDto>> GetTreeAsync()
    {
        var allItems = await unitOfWork.Repository<T3Item>().ListAsync(ItemSpec.Tree());
        var itemDict = allItems.ToDictionary(i => i.Id);
        var tree = new List<ItemTreeDto>();

        foreach (var item in allItems.Where(i => i.ListParents.Count == 0))
            tree.Add(BuildTree(item, 0));

        ItemTreeDto BuildTree(T3Item item, int level)
        {
            var dto = mapper.Map<ItemTreeDto>(item);
            dto.Level = level;
            dto.Children = [.. item.ListChilds
                .Select(c => itemDict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))];
            return dto;
        }

        return tree;
    }

    public async Task<List<ItemHierarchyDto>> GetFlatHierarchyAsync(Guid itemId)
    {
        var item = await unitOfWork.Repository<T3Item>().GetByIdAsync(itemId, i => i.ListParents, i => i.ListChilds);
        if (item == null) return [];

        var hierarchies = item.ListParents.Concat(item.ListChilds).ToList();
        return mapper.Map<List<ItemHierarchyDto>>(hierarchies);
    }

    public async Task AddAsync(ItemDto dto)
    {
        var entity = mapper.Map<T3Item>(dto);
        await unitOfWork.Repository<T3Item>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(ItemDto dto)
    {
        var entity = mapper.Map<T3Item>(dto);
        await unitOfWork.Repository<T3Item>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Item>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3ItemHierarchy>();
        bool exists = await repo.AnyAsync(h => h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3ItemHierarchy
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
        var repo = unitOfWork.Repository<T3ItemHierarchy>();
        var relations = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var relation in relations)
            await repo.DeleteAsync(relation.Id);

        await unitOfWork.SaveChangesAsync();
    }
}
