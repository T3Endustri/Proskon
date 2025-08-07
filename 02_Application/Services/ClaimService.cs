using _01_Data.Entities;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ClaimService(
    IGenericService<T3IdentityClaim> claimService,
    IMapper mapper
) : IClaimService
{
    public async Task<List<ClaimDto>> GetAllAsync()
    {
        var claims = await claimService.ListAsync(ClaimSpec.All());
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task<List<ClaimDto>> GetByUserIdAsync(Guid userId)
    {
        var claims = await claimService.ListAsync(ClaimSpec.ByUserId(userId));
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task<List<ClaimDto>> GetByRoleIdAsync(Guid roleId)
    {
        var claims = await claimService.ListAsync(ClaimSpec.ByRoleId(roleId));
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task AssignClaimsToUserAsync(ClaimAssignDto dto)
    {
        foreach (var claim in dto.Claims)
        {
            var exists = await claimService.AnyAsync(c =>
                c.UserId == dto.UserId &&
                c.Type == claim.Type &&
                c.Value == claim.Value);

            if (!exists)
            {
                var entity = mapper.Map<T3IdentityClaim>(claim);
                entity.Id = Guid.NewGuid();
                entity.UserId = dto.UserId;
                await claimService.AddAsync(entity);
            }
        }
    }

    public async Task AssignClaimsToRoleAsync(ClaimAssignDto dto)
    {
        foreach (var claim in dto.Claims)
        {
            var exists = await claimService.AnyAsync(c =>
                c.RoleId == dto.RoleId &&
                c.Type == claim.Type &&
                c.Value == claim.Value);

            if (!exists)
            {
                var entity = mapper.Map<T3IdentityClaim>(claim);
                entity.Id = Guid.NewGuid();
                entity.RoleId = dto.RoleId;
                await claimService.AddAsync(entity);
            }
        }
    }

    public async Task DeleteAsync(Guid claimId)
    {
        await claimService.DeleteAsync(claimId);
    }
}
