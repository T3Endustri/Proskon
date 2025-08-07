using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IPropertyService
{
    Task<List<PropertyListDto>> GetAllAsync();
    Task<PropertyDto?> GetByIdAsync(Guid id);
    Task<List<PropertyListDto>> SearchAsync(string keyword);
    Task<List<PropertyListDto>> GetByFormResourceAsync(Guid resourceId);
    Task<List<PropertyDto>> GetRequiredAsync();
    Task AddAsync(PropertyDto dto);
    Task UpdateAsync(PropertyDto dto);
    Task DeleteAsync(Guid id);
}
