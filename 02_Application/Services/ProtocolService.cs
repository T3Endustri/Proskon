using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ProtocolService(IUnitOfWork unitOfWork, IMapper mapper) : IProtocolService
{
    public async Task<List<ProtocolListDto>> GetAllAsync()
    {
        var protocols = await unitOfWork.Repository<T3Protocol>().ListAsync(ProtocolSpec.All());
        return mapper.Map<List<ProtocolListDto>>(protocols);
    }

    public async Task<ProtocolDto?> GetByIdAsync(Guid id)
    {
        var protocol = await unitOfWork.Repository<T3Protocol>().GetByIdAsync(id, p => p.ProcessType, p => p.ListProtocolItems);
        return protocol is null ? null : mapper.Map<ProtocolDto>(protocol);
    }

    public async Task<List<ProtocolListDto>> GetByProcessTypeAsync(Guid processTypeId)
    {
        var protocols = await unitOfWork.Repository<T3Protocol>().ListAsync(ProtocolSpec.ByProcessType(processTypeId));
        return mapper.Map<List<ProtocolListDto>>(protocols);
    }

    public async Task AddAsync(ProtocolDto dto)
    {
        var entity = mapper.Map<T3Protocol>(dto);
        await unitOfWork.Repository<T3Protocol>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProtocolDto dto)
    {
        var entity = mapper.Map<T3Protocol>(dto);
        await unitOfWork.Repository<T3Protocol>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Protocol>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
