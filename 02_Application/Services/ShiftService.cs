using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ShiftService(
    IGenericService<T3Shift> shiftService,
    IMapper mapper
) : IShiftService
{
    public async Task<List<ShiftListDto>> GetAllAsync()
    {
        var shifts = await shiftService.ListAsync(ShiftSpec.All());
        return mapper.Map<List<ShiftListDto>>(shifts);
    }

    public async Task<ShiftDto?> GetByIdAsync(Guid id)
    {
        var shift = await shiftService.GetByIdAsync(id, s => s.ListBreaks, s => s.Location);
        return mapper.Map<ShiftDto>(shift);
    }

    public async Task<List<ShiftListDto>> GetByLocationAsync(Guid locationId)
    {
        var shifts = await shiftService.ListAsync(ShiftSpec.ByLocation(locationId));
        return mapper.Map<List<ShiftListDto>>(shifts);
    }

    public async Task AddAsync(ShiftDto dto)
    {
        var entity = mapper.Map<T3Shift>(dto);
        await shiftService.AddAsync(entity);
    }

    public async Task UpdateAsync(ShiftDto dto)
    {
        var entity = mapper.Map<T3Shift>(dto);
        await shiftService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await shiftService.DeleteAsync(id);
    }
}
