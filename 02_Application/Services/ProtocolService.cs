using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ProtocolService(
    IGenericService<T3Protocol> protocolService,
    IMapper mapper
) : IProtocolService
{
    public async Task<List<ProtocolListDto>> GetAllAsync()
    {
        var protocols = await protocolService.ListAsync(ProtocolSpec.All());
        return mapper.Map<List<ProtocolListDto>>(protocols);
    }

    public async Task<ProtocolDto?> GetByIdAsync(Guid id)
    {
        var protocol = await protocolService.GetByIdAsync(id, p => p.ProcessType, p => p.ListProtocolItems);
        return mapper.Map<ProtocolDto>(protocol);
    }

    public async Task<List<ProtocolListDto>> GetByProcessTypeAsync(Guid processTypeId)
    {
        var protocols = await protocolService.ListAsync(ProtocolSpec.ByProcessType(processTypeId));
        return mapper.Map<List<ProtocolListDto>>(protocols);
    }

    public async Task AddAsync(ProtocolDto dto)
    {
        var entity = mapper.Map<T3Protocol>(dto);
        await protocolService.AddAsync(entity);
    }

    public async Task UpdateAsync(ProtocolDto dto)
    {
        var entity = mapper.Map<T3Protocol>(dto);
        await protocolService.UpdateAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        await protocolService.DeleteAsync(id);
    }
}
