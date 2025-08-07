using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ShiftService(IUnitOfWork unitOfWork, IMapper mapper) : IShiftService
{
    public async Task<List<ShiftListDto>> GetAllAsync()
    {
        var shifts = await unitOfWork.Repository<T3Shift>().ListAsync(ShiftSpec.All());
        return mapper.Map<List<ShiftListDto>>(shifts);
    }

    public async Task<ShiftDto?> GetByIdAsync(Guid id)
    {
        var shift = await unitOfWork.Repository<T3Shift>().GetByIdAsync(id, s => s.ListBreaks, s => s.Location);
        return shift is null ? null : mapper.Map<ShiftDto>(shift);
    }

    public async Task<List<ShiftListDto>> GetByLocationAsync(Guid locationId)
    {
        var shifts = await unitOfWork.Repository<T3Shift>().ListAsync(ShiftSpec.ByLocation(locationId));
        return mapper.Map<List<ShiftListDto>>(shifts);
    }

    public async Task AddAsync(ShiftDto dto)
    {
        var entity = mapper.Map<T3Shift>(dto);
        await unitOfWork.Repository<T3Shift>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftDto dto)
    {
        var entity = mapper.Map<T3Shift>(dto);
        await unitOfWork.Repository<T3Shift>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Shift>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
