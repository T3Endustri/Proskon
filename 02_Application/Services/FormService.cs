using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class FormService(
    IGenericService<T3Form> formService,
    IMapper mapper
) : IFormService
{
    public async Task<List<FormListDto>> GetAllAsync()
    {
        var forms = await formService.ListAsync(FormSpec.All());
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<FormDto?> GetByIdAsync(Guid id)
    {
        var forms = await formService.ListAsync(FormSpec.ById(id));
        return mapper.Map<FormDto>(forms.FirstOrDefault());
    }

    public async Task<List<FormListDto>> GetByUserAsync(Guid userId)
    {
        var forms = await formService.ListAsync(FormSpec.ByUser(userId));
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<List<FormListDto>> GetByTemplateAsync(Guid templateId)
    {
        var forms = await formService.ListAsync(FormSpec.ByTemplate(templateId));
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<List<FormListDto>> GetUnapprovedAsync()
    {
        var forms = await formService.ListAsync(FormSpec.Unapproved());
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<(List<FormListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        var (forms, total) = await formService.PagingAsync(
            f => f.ListFormFields.Any(x => x.PropertyField.Name.Contains(keyword)),
            selector: form => mapper.Map<FormListDto>(form),
            orderBy: f => f.CreateTime,
            descending: true,
            skip,
            take
        );

        return (forms, total);
    }

    public async Task AddAsync(FormDto dto)
    {
        var entity = mapper.Map<T3Form>(dto);
        await formService.AddAsync(entity);
    }

    public async Task UpdateAsync(FormDto dto)
    {
        var entity = mapper.Map<T3Form>(dto);
        await formService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await formService.DeleteAsync(id);
    }
}
