using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class TemplateService(
    IGenericService<T3Template> templateService,
    IMapper mapper
) : ITemplateService
{
    public async Task<List<TemplateListDto>> GetAllAsync()
    {
        var templates = await templateService.ListAsync(TemplateSpec.All());
        return mapper.Map<List<TemplateListDto>>(templates);
    }

    public async Task<TemplateDto?> GetByIdAsync(Guid id)
    {
        var templates = await templateService.ListAsync(TemplateSpec.ById(id));
        return mapper.Map<TemplateDto>(templates.FirstOrDefault());
    }

    public async Task<List<TemplateListDto>> GetByApproverAsync(Guid userId)
    {
        var templates = await templateService.ListAsync(TemplateSpec.HasApprover(userId));
        return mapper.Map<List<TemplateListDto>>(templates);
    }

    public async Task AddAsync(TemplateDto dto)
    {
        var entity = mapper.Map<T3Template>(dto);
        await templateService.AddAsync(entity);
    }

    public async Task UpdateAsync(TemplateDto dto)
    {
        var entity = mapper.Map<T3Template>(dto);
        await templateService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await templateService.DeleteAsync(id);
    }
}
