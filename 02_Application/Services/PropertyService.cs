using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class PropertyService(IUnitOfWork unitOfWork, IMapper mapper) : IPropertyService
{
    public async Task<List<PropertyListDto>> GetAllAsync()
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.All());
        return mapper.Map<List<PropertyListDto>>(props);
    }

    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        var prop = await unitOfWork.Repository<T3Property>().GetByIdAsync(id, p => p.ListPanels, p => p.ListTemplates);
        return prop is null ? null : mapper.Map<PropertyDto>(prop);
    }

    public async Task<List<PropertyListDto>> SearchAsync(string keyword)
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.Search(keyword));
        return mapper.Map<List<PropertyListDto>>(props);
    }

    public async Task<List<PropertyListDto>> GetByFormResourceAsync(Guid resourceId)
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.ByFormResource(resourceId));
        return mapper.Map<List<PropertyListDto>>(props);
    }

    public async Task<List<PropertyDto>> GetRequiredAsync()
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.IsRequired());
        return mapper.Map<List<PropertyDto>>(props);
    }

    public async Task AddAsync(PropertyDto dto)
    {
        var entity = mapper.Map<T3Property>(dto);
        await unitOfWork.Repository<T3Property>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(PropertyDto dto)
    {
        var entity = mapper.Map<T3Property>(dto);
        await unitOfWork.Repository<T3Property>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Property>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
