using _01_Data.Entities;
namespace _01_Data.Specifications;

public static class RoleSpec
{
    public static ISpecification<T3IdentityRole> All()
        => new InlineSpec<T3IdentityRole>();

    public static ISpecification<T3IdentityRole> Teams()
        => new InlineSpec<T3IdentityRole>(r => r.IsTeam && r.IsActive)
        {
            OrderBy = r => r.Name,
            Includes = [r => r.ListUsers]
        };

    public static ISpecification<T3IdentityRole> Departments()
        => new InlineSpec<T3IdentityRole>(r => r.IsDepartment && r.IsActive)
        {
            OrderBy = r => r.Name,
            Includes = [r => r.ListUsers]
        };

    public static ISpecification<T3IdentityRole> ById(Guid id)
        => new InlineSpec<T3IdentityRole>(r => r.Id == id)
        {
            Includes = [r => r.ListClaims, r => r.ListUsers]
        };

    public static ISpecification<T3IdentityRole> ByUserId(Guid userId)
        => new InlineSpec<T3IdentityRole>(r => r.ListUsers.Any(u => u.UserId == userId))
        {
            Includes = [r => r.ListUsers]
        };

    public static ISpecification<T3IdentityRole> SearchPaged(string keyword, int skip, int take)
        => new InlineSpec<T3IdentityRole>(r => r.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            OrderBy = r => r.Name,
            Skip = skip,
            Take = take
        };
}