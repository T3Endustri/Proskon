using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class FormResourceService(
    IGenericService<T3FormResource> resourceService,
    IMapper mapper
) : IFormResourceService
{
    public async Task<List<FormResourceListDto>> GetAllAsync()
    {
        var resources = await resourceService.ListAsync(FormResourceSpec.All());
        return mapper.Map<List<FormResourceListDto>>(resources);
    }

    public async Task<FormResourceDto?> GetByIdAsync(Guid id)
    {
        var resource = await resourceService.GetByIdAsync(id, r => r.ListItems);
        return mapper.Map<FormResourceDto>(resource);
    }
     
    public async Task AddAsync(FormResourceDto dto)
    {
        var entity = mapper.Map<T3FormResource>(dto);
        await resourceService.AddAsync(entity);
    }

    public async Task UpdateAsync(FormResourceDto dto)
    {
        var entity = mapper.Map<T3FormResource>(dto);
        await resourceService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await resourceService.DeleteAsync(id);
    }
}
