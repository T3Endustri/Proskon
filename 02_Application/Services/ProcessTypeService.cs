using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ProcessTypeService(
    IGenericService<T3ProcessType> typeService,
    IGenericService<T3ProcessTypeItem> itemLinkService,
    IGenericService<T3ProcessTypeModule> moduleLinkService,
    IMapper mapper
) : IProcessTypeService
{
    public async Task<List<ProcessTypeListDto>> GetAllAsync()
    {
        var types = await typeService.ListAsync(ProcessTypeSpec.All());
        return mapper.Map<List<ProcessTypeListDto>>(types);
    }

    public async Task<ProcessTypeDto?> GetByIdAsync(Guid id)
    {
        var types = await typeService.ListAsync(ProcessTypeSpec.ById(id));
        return mapper.Map<ProcessTypeDto>(types.FirstOrDefault());
    }

    public async Task<List<ProcessTypeListDto>> SearchAsync(string keyword)
    {
        var types = await typeService.ListAsync(ProcessTypeSpec.Search(keyword));
        return mapper.Map<List<ProcessTypeListDto>>(types);
    }

    public async Task<(List<ProcessTypeListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        var (items, total) = await typeService.PagingAsync(
            p => p.Name.Contains(keyword) || p.Barcode.Contains(keyword),
            selector: type => mapper.Map<ProcessTypeListDto>(type),
            orderBy: p => p.Name,
            descending: false,
            skip,
            take
        );
        return (items, total);
    }

    public async Task AddAsync(ProcessTypeDto dto)
    {
        var entity = mapper.Map<T3ProcessType>(dto);
        await typeService.AddAsync(entity);
    }

    public async Task UpdateAsync(ProcessTypeDto dto)
    {
        var entity = mapper.Map<T3ProcessType>(dto);
        await typeService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await typeService.DeleteAsync(id);
    }

    public async Task AssignItemAsync(Guid typeId, Guid itemId)
    {
        var exists = await itemLinkService.AnyAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        if (!exists)
        {
            await itemLinkService.AddAsync(new T3ProcessTypeItem
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ItemId = itemId
            });
        }
    }

    public async Task AssignModuleAsync(Guid typeId, Guid moduleId)
    {
        var exists = await moduleLinkService.AnyAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        if (!exists)
        {
            await moduleLinkService.AddAsync(new T3ProcessTypeModule
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ModuleId = moduleId
            });
        }
    }

    public async Task RemoveItemAsync(Guid typeId, Guid itemId)
    {
        var items = await itemLinkService.WhereAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        foreach (var i in items)
            await itemLinkService.DeleteAsync(i.Id);
    }

    public async Task RemoveModuleAsync(Guid typeId, Guid moduleId)
    {
        var modules = await moduleLinkService.WhereAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        foreach (var m in modules)
            await moduleLinkService.DeleteAsync(m.Id);
    }
}
