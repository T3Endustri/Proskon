using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IShiftTypeService
{
    Task<List<ShiftTypeListDto>> GetAllAsync();
    Task<ShiftTypeDto?> GetByIdAsync(Guid id);
    Task AddAsync(ShiftTypeDto dto);
    Task UpdateAsync(ShiftTypeDto dto);
    Task DeleteAsync(Guid id);
}
