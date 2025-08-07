using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IProcessTypeService
{
    Task<List<ProcessTypeListDto>> GetAllAsync();
    Task<ProcessTypeDto?> GetByIdAsync(Guid id);
    Task<List<ProcessTypeListDto>> SearchAsync(string keyword);
    Task<(List<ProcessTypeListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take);
    Task AddAsync(ProcessTypeDto dto);
    Task UpdateAsync(ProcessTypeDto dto);
    Task DeleteAsync(Guid id);

    Task AssignItemAsync(Guid typeId, Guid itemId);
    Task AssignModuleAsync(Guid typeId, Guid moduleId);
    Task RemoveItemAsync(Guid typeId, Guid itemId);
    Task RemoveModuleAsync(Guid typeId, Guid moduleId);
}
