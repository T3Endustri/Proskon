using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IFormService
{
    Task<List<FormListDto>> GetAllAsync();
    Task<FormDto?> GetByIdAsync(Guid id);
    Task<List<FormListDto>> GetByUserAsync(Guid userId);
    Task<List<FormListDto>> GetByTemplateAsync(Guid templateId);
    Task<List<FormListDto>> GetUnapprovedAsync();
    Task<(List<FormListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take);
    Task AddAsync(FormDto dto);
    Task UpdateAsync(FormDto dto);
    Task DeleteAsync(Guid id);
}
