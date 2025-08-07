using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IFormResourceService
{
    Task<List<FormResourceListDto>> GetAllAsync();
    Task<FormResourceDto?> GetByIdAsync(Guid id); 
    Task AddAsync(FormResourceDto dto);
    Task UpdateAsync(FormResourceDto dto);
    Task DeleteAsync(Guid id);
}
