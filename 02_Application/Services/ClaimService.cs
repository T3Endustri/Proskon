using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class ClaimService(IUnitOfWork unitOfWork, IMapper mapper) : IClaimService
{
    public async Task<List<ClaimDto>> GetAllAsync()
    {
        var spec = ClaimSpec.All();
        var claims = await unitOfWork.Repository<T3IdentityClaim>().ListAsync(spec);
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task<List<ClaimDto>> GetByUserIdAsync(Guid userId)
    {
        var spec = ClaimSpec.ByUserId(userId);
        var claims = await unitOfWork.Repository<T3IdentityClaim>().ListAsync(spec);
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task<List<ClaimDto>> GetByRoleIdAsync(Guid roleId)
    {
        var spec = ClaimSpec.ByRoleId(roleId);
        var claims = await unitOfWork.Repository<T3IdentityClaim>().ListAsync(spec);
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task AssignClaimsToUserAsync(ClaimAssignDto dto)
    {
        if (dto.UserId is null) return;

        var repo = unitOfWork.Repository<T3IdentityClaim>();

        var existingClaims = await repo.WhereAsync(c => c.UserId == dto.UserId);
        foreach (var claim in existingClaims)
            await repo.DeleteAsync(claim.Id);

        foreach (var newClaim in dto.Claims)
        {
            var claim = new T3IdentityClaim
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Type = newClaim.Type,
                Value = newClaim.Value,
                PermissionType = newClaim.PermissionType
            };

            await repo.AddAsync(claim);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task AssignClaimsToRoleAsync(ClaimAssignDto dto)
    {
        if (dto.RoleId is null) return;

        var repo = unitOfWork.Repository<T3IdentityClaim>();

        var existingClaims = await repo.WhereAsync(c => c.RoleId == dto.RoleId);
        foreach (var claim in existingClaims)
            await repo.DeleteAsync(claim.Id);

        foreach (var newClaim in dto.Claims)
        {
            var claim = new T3IdentityClaim
            {
                Id = Guid.NewGuid(),
                RoleId = dto.RoleId,
                Type = newClaim.Type,
                Value = newClaim.Value,
                PermissionType = newClaim.PermissionType
            };

            await repo.AddAsync(claim);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid claimId)
    {
        await unitOfWork.Repository<T3IdentityClaim>().DeleteAsync(claimId);
        await unitOfWork.SaveChangesAsync();
    }
}
