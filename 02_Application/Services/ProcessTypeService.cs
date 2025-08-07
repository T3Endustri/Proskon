using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ProcessTypeService(IUnitOfWork unitOfWork, IMapper mapper) : IProcessTypeService
{
    public async Task<List<ProcessTypeListDto>> GetAllAsync()
    {
        var types = await unitOfWork.Repository<T3ProcessType>().ListAsync(ProcessTypeSpec.All());
        return mapper.Map<List<ProcessTypeListDto>>(types);
    }

    public async Task<ProcessTypeDto?> GetByIdAsync(Guid id)
    {
        var types = await unitOfWork.Repository<T3ProcessType>().ListAsync(ProcessTypeSpec.ById(id));
        return types.Count == 0 ? null : mapper.Map<ProcessTypeDto>(types[0]);
    }

    public async Task<List<ProcessTypeListDto>> SearchAsync(string keyword)
    {
        var types = await unitOfWork.Repository<T3ProcessType>().ListAsync(ProcessTypeSpec.Search(keyword));
        return mapper.Map<List<ProcessTypeListDto>>(types);
    }

    public async Task<(List<ProcessTypeListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        var repo = unitOfWork.Repository<T3ProcessType>();
        var (items, total) = await repo.PagingAsync(
            p => p.Name.Contains(keyword) || p.Barcode.Contains(keyword),
            selector: p => mapper.Map<ProcessTypeListDto>(p),
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
        await unitOfWork.Repository<T3ProcessType>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProcessTypeDto dto)
    {
        var entity = mapper.Map<T3ProcessType>(dto);
        await unitOfWork.Repository<T3ProcessType>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3ProcessType>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AssignItemAsync(Guid typeId, Guid itemId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeItem>();
        var exists = await repo.AnyAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        if (!exists)
        {
            await repo.AddAsync(new T3ProcessTypeItem
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ItemId = itemId
            });
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task AssignModuleAsync(Guid typeId, Guid moduleId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeModule>();
        var exists = await repo.AnyAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        if (!exists)
        {
            await repo.AddAsync(new T3ProcessTypeModule
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ModuleId = moduleId
            });
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveItemAsync(Guid typeId, Guid itemId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeItem>();
        var items = await repo.WhereAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        foreach (var i in items)
            await repo.DeleteAsync(i.Id);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveModuleAsync(Guid typeId, Guid moduleId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeModule>();
        var modules = await repo.WhereAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        foreach (var m in modules)
            await repo.DeleteAsync(m.Id);

        await unitOfWork.SaveChangesAsync();
    }
}