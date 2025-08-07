using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IShiftService
{
    Task<List<ShiftListDto>> GetAllAsync();
    Task<ShiftDto?> GetByIdAsync(Guid id);
    Task<List<ShiftListDto>> GetByLocationAsync(Guid locationId);
    Task AddAsync(ShiftDto dto);
    Task UpdateAsync(ShiftDto dto);
    Task DeleteAsync(Guid id);
}
