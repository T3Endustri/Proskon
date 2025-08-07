using _01_Data.Entities;

namespace _02_Application.Interfaces;

public interface IRoleService
{
    Task<List<T3IdentityRole>> GetAllChildRolesRecursiveAsync(Guid roleId);
    Task<List<T3IdentityRole>> GetAllParentRolesRecursiveAsync(Guid roleId);
}