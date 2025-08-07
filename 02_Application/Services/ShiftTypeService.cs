using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ShiftTypeService(IUnitOfWork unitOfWork, IMapper mapper) : IShiftTypeService
{
    public async Task<List<ShiftTypeListDto>> GetAllAsync()
    {
        var types = await unitOfWork.Repository<T3ShiftType>().ListAsync(ShiftTypeSpec.All());
        return mapper.Map<List<ShiftTypeListDto>>(types);
    }

    public async Task<ShiftTypeDto?> GetByIdAsync(Guid id)
    {
        var types = await unitOfWork.Repository<T3ShiftType>().ListAsync(ShiftTypeSpec.ById(id));
        return types.Count == 0 ? null : mapper.Map<ShiftTypeDto>(types[0]);
    }

    public async Task AddAsync(ShiftTypeDto dto)
    {
        var entity = mapper.Map<T3ShiftType>(dto);
        await unitOfWork.Repository<T3ShiftType>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftTypeDto dto)
    {
        var entity = mapper.Map<T3ShiftType>(dto);
        await unitOfWork.Repository<T3ShiftType>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3ShiftType>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
