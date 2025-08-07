using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ShiftTypeService(
    IGenericService<T3ShiftType> shiftTypeService,
    IMapper mapper
) : IShiftTypeService
{
    public async Task<List<ShiftTypeListDto>> GetAllAsync()
    {
        var types = await shiftTypeService.ListAsync(ShiftTypeSpec.All());
        return mapper.Map<List<ShiftTypeListDto>>(types);
    }

    public async Task<ShiftTypeDto?> GetByIdAsync(Guid id)
    {
        var type = await shiftTypeService.ListAsync(ShiftTypeSpec.ById(id));
        return mapper.Map<ShiftTypeDto>(type.FirstOrDefault());
    }

    public async Task AddAsync(ShiftTypeDto dto)
    {
        var entity = mapper.Map<T3ShiftType>(dto);
        await shiftTypeService.AddAsync(entity);
    }

    public async Task UpdateAsync(ShiftTypeDto dto)
    {
        var entity = mapper.Map<T3ShiftType>(dto);
        await shiftTypeService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await shiftTypeService.DeleteAsync(id);
    }
}
