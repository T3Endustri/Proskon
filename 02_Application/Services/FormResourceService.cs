using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class FormResourceService(IUnitOfWork unitOfWork, IMapper mapper) : IFormResourceService
{
    public async Task<List<FormResourceListDto>> GetAllAsync()
    {
        var spec = FormResourceSpec.All();
        var resources = await unitOfWork.Repository<T3FormResource>().ListAsync(spec);
        return mapper.Map<List<FormResourceListDto>>(resources);
    }

    public async Task<FormResourceDto?> GetByIdAsync(Guid id)
    {
        var resource = await unitOfWork.Repository<T3FormResource>().GetByIdAsync(id, r => r.ListItems);
        return resource is null ? null : mapper.Map<FormResourceDto>(resource);
    }

    public async Task AddAsync(FormResourceDto dto)
    {
        var entity = mapper.Map<T3FormResource>(dto);
        await unitOfWork.Repository<T3FormResource>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(FormResourceDto dto)
    {
        var entity = mapper.Map<T3FormResource>(dto);
        await unitOfWork.Repository<T3FormResource>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3FormResource>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
