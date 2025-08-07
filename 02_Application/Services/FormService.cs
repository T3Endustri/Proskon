using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class FormService(IUnitOfWork unitOfWork, IMapper mapper) : IFormService
{
    public async Task<List<FormListDto>> GetAllAsync()
    {
        var spec = FormSpec.All();
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<FormDto?> GetByIdAsync(Guid id)
    {
        var spec = FormSpec.ById(id);
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return forms.Count == 0 ? null : mapper.Map<FormDto>(forms[0]);
    }

    public async Task<List<FormListDto>> GetByUserAsync(Guid userId)
    {
        var spec = FormSpec.ByUser(userId);
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<List<FormListDto>> GetByTemplateAsync(Guid templateId)
    {
        var spec = FormSpec.ByTemplate(templateId);
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<List<FormListDto>> GetUnapprovedAsync()
    {
        var spec = FormSpec.Unapproved();
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }

    public async Task<(List<FormListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        var repo = unitOfWork.Repository<T3Form>();

        var (items, total) = await repo.PagingAsync(
            predicate: f => f.ListFormFields.Any(x => x.PropertyField.Name.Contains(keyword)),
            selector: f => mapper.Map<FormListDto>(f),
            orderBy: f => f.CreateTime,
            descending: true,
            skip,
            take
        );

        return (items, total);
    }

    public async Task AddAsync(FormDto dto)
    {
        var entity = mapper.Map<T3Form>(dto);
        await unitOfWork.Repository<T3Form>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(FormDto dto)
    {
        var entity = mapper.Map<T3Form>(dto);
        await unitOfWork.Repository<T3Form>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Form>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
