using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class TemplateService(IUnitOfWork unitOfWork, IMapper mapper) : ITemplateService
{
    public async Task<List<TemplateListDto>> GetAllAsync()
    {
        var templates = await unitOfWork.Repository<T3Template>().ListAsync(TemplateSpec.All());
        return mapper.Map<List<TemplateListDto>>(templates);
    }

    public async Task<TemplateDto?> GetByIdAsync(Guid id)
    {
        var templates = await unitOfWork.Repository<T3Template>().ListAsync(TemplateSpec.ById(id));
        return templates.Count == 0 ? null : mapper.Map<TemplateDto>(templates[0]);
    }

    public async Task<List<TemplateListDto>> GetByApproverAsync(Guid userId)
    {
        var templates = await unitOfWork.Repository<T3Template>().ListAsync(TemplateSpec.HasApprover(userId));
        return mapper.Map<List<TemplateListDto>>(templates);
    }

    public async Task AddAsync(TemplateDto dto)
    {
        var entity = mapper.Map<T3Template>(dto);
        await unitOfWork.Repository<T3Template>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(TemplateDto dto)
    {
        var entity = mapper.Map<T3Template>(dto);
        await unitOfWork.Repository<T3Template>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Template>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}