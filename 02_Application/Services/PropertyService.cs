using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class PropertyService(
    IGenericService<T3Property> propertyService,
    IMapper mapper
) : IPropertyService
{
    public async Task<List<PropertyListDto>> GetAllAsync()
    {
        var props = await propertyService.ListAsync(PropertySpec.All());
        return mapper.Map<List<PropertyListDto>>(props);
    }

    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        var prop = await propertyService.GetByIdAsync(id, p => p.ListPanels, p => p.ListTemplates);
        return mapper.Map<PropertyDto>(prop);
    }

    public async Task<List<PropertyListDto>> SearchAsync(string keyword)
    {
        var props = await propertyService.ListAsync(PropertySpec.Search(keyword));
        return mapper.Map<List<PropertyListDto>>(props);
    }

    public async Task<List<PropertyListDto>> GetByFormResourceAsync(Guid resourceId)
    {
        var props = await propertyService.ListAsync(PropertySpec.ByFormResource(resourceId));
        return mapper.Map<List<PropertyListDto>>(props);
    }

    public async Task<List<PropertyDto>> GetRequiredAsync()
    {
        var props = await propertyService.ListAsync(PropertySpec.IsRequired());
        return mapper.Map<List<PropertyDto>>(props);
    }

    public async Task AddAsync(PropertyDto dto)
    {
        var entity = mapper.Map<T3Property>(dto);
        await propertyService.AddAsync(entity);
    }

    public async Task UpdateAsync(PropertyDto dto)
    {
        var entity = mapper.Map<T3Property>(dto);
        await propertyService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await propertyService.DeleteAsync(id);
    }
}
