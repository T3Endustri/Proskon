using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IProtocolService
{
    Task<List<ProtocolListDto>> GetAllAsync();
    Task<ProtocolDto?> GetByIdAsync(Guid id);
    Task<List<ProtocolListDto>> GetByProcessTypeAsync(Guid processTypeId);
    Task AddAsync(ProtocolDto dto);
    Task UpdateAsync(ProtocolDto dto);
    Task DeleteAsync(Guid id);
}
