using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface ITemplateService
{
    Task<List<TemplateListDto>> GetAllAsync();
    Task<TemplateDto?> GetByIdAsync(Guid id);
    Task<List<TemplateListDto>> GetByApproverAsync(Guid userId);
    Task AddAsync(TemplateDto dto);
    Task UpdateAsync(TemplateDto dto);
    Task DeleteAsync(Guid id);
}
