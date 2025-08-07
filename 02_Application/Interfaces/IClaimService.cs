using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IClaimService
{
    Task<List<ClaimDto>> GetAllAsync();
    Task<List<ClaimDto>> GetByUserIdAsync(Guid userId);
    Task<List<ClaimDto>> GetByRoleIdAsync(Guid roleId);
    Task AssignClaimsToUserAsync(ClaimAssignDto dto);
    Task AssignClaimsToRoleAsync(ClaimAssignDto dto);
    Task DeleteAsync(Guid claimId);
}
