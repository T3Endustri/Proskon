using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class RoleService(IUnitOfWork unitOfWork, IMapper mapper) : IRoleService
{
    public async Task<List<RoleListDto>> GetAllAsync()
    {
        var roles = await unitOfWork.Repository<T3IdentityRole>().ListAsync(RoleSpec.All());
        return mapper.Map<List<RoleListDto>>(roles);
    }

    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        var role = await unitOfWork.Repository<T3IdentityRole>().GetByIdAsync(id, r => r.ListClaims, r => r.ListUsers);
        return role is null ? null : mapper.Map<RoleDto>(role);
    }

    public async Task<List<RoleListDto>> SearchAsync(string keyword)
    {
        var roles = await unitOfWork.Repository<T3IdentityRole>().ListAsync(RoleSpec.SearchPaged(keyword, 0, int.MaxValue));
        return mapper.Map<List<RoleListDto>>(roles);
    }

    public async Task<(List<RoleListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        var spec = RoleSpec.SearchPaged(keyword, skip, take);
        var items = await unitOfWork.Repository<T3IdentityRole>().ListAsync(spec);
        var total = await unitOfWork.Repository<T3IdentityRole>().CountAsync(r => r.Name.Contains(keyword));
        return (mapper.Map<List<RoleListDto>>(items), total);
    }

    public async Task<List<RoleDto>> GetByUserIdAsync(Guid userId)
    {
        var roles = await unitOfWork.Repository<T3IdentityRole>().ListAsync(RoleSpec.ByUserId(userId));
        return mapper.Map<List<RoleDto>>(roles);
    }

    public async Task<List<RoleTreeDto>> GetTreeAsync()
    {
        var all = await unitOfWork.Repository<T3IdentityRole>().ListAsync(RoleSpec.All());
        var dict = all.ToDictionary(r => r.Id);
        var tree = new List<RoleTreeDto>();

        foreach (var root in all.Where(r => r.ListParents.Count == 0))
            tree.Add(BuildTree(root, 0));

        RoleTreeDto BuildTree(T3IdentityRole role, int level)
        {
            var dto = mapper.Map<RoleTreeDto>(role);
            dto.Level = level;
            dto.Children = role.ListChilds
                .Select(c => dict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))
                .ToList();
            return dto;
        }

        return tree;
    }

    public async Task AddAsync(RoleDto dto)
    {
        var entity = mapper.Map<T3IdentityRole>(dto);
        await unitOfWork.Repository<T3IdentityRole>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoleDto dto)
    {
        var entity = mapper.Map<T3IdentityRole>(dto);
        await unitOfWork.Repository<T3IdentityRole>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3IdentityRole>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<List<T3IdentityRole>> GetAllChildRolesRecursiveAsync(Guid roleId)
    {
        var allRoles = await unitOfWork.Repository<T3IdentityRole>().GetAllAsync(r => r.ListChilds);
        var visited = new HashSet<Guid>();
        var result = new List<T3IdentityRole>();

        void Traverse(Guid id)
        {
            if (!visited.Add(id)) return;

            var current = allRoles.FirstOrDefault(r => r.Id == id);
            if (current is null) return;

            foreach (var childRelation in current.ListChilds)
            {
                if (!visited.Contains(childRelation.ChildId))
                {
                    var child = allRoles.FirstOrDefault(r => r.Id == childRelation.ChildId);
                    if (child is not null)
                    {
                        result.Add(child);
                        Traverse(child.Id);
                    }
                }
            }
        }

        Traverse(roleId);
        return result;
    }

    public async Task<List<T3IdentityRole>> GetAllParentRolesRecursiveAsync(Guid roleId)
    {
        var allRoles = await unitOfWork.Repository<T3IdentityRole>().GetAllAsync(r => r.ListParents);
        var visited = new HashSet<Guid>();
        var result = new List<T3IdentityRole>();

        void Traverse(Guid id)
        {
            if (!visited.Add(id)) return;

            var current = allRoles.FirstOrDefault(r => r.Id == id);
            if (current is null) return;

            foreach (var parentRelation in current.ListParents)
            {
                if (!visited.Contains(parentRelation.ParentId))
                {
                    var parent = allRoles.FirstOrDefault(r => r.Id == parentRelation.ParentId);
                    if (parent is not null)
                    {
                        result.Add(parent);
                        Traverse(parent.Id);
                    }
                }
            }
        }

        Traverse(roleId);
        return result;
    }
}
