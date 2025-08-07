using _01_Data.Entities;
using _02_Application.Interfaces;

namespace _02_Application.Services;

public class RoleService(IGenericService<T3IdentityRole> roleService) : IRoleService
{
    public async Task<List<T3IdentityRole>> GetAllChildRolesRecursiveAsync(Guid roleId)
    {
        var allRoles = await roleService.GetAllAsync(r => r.ListChilds);
        var visited = new HashSet<Guid>();
        var result = new List<T3IdentityRole>();

        void Traverse(Guid id)
        {
            if (visited.Contains(id)) return;
            visited.Add(id);

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
        var allRoles = await roleService.GetAllAsync(r => r.ListParents);
        var visited = new HashSet<Guid>();
        var result = new List<T3IdentityRole>();

        void Traverse(Guid id)
        {
            if (visited.Contains(id)) return;
            visited.Add(id);

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