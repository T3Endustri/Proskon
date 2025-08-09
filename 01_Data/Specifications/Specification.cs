using _01_Data.Entities;
using System.Linq.Expressions;
namespace _01_Data.Specifications;

#region Bases
public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>[] Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    bool IsDescending { get; }
    int? Skip { get; }
    int? Take { get; }
}
public class InlineSpec<T> : BaseSpecification<T>
{
    public InlineSpec(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }
    public InlineSpec() : this(_ => true)
    {
    }
    public InlineSpec(params Expression<Func<T, object>>[] includes) : this(_ => true)
    {
        Includes = includes;
    }
}
public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; set; }
    public Expression<Func<T, object>>[] Includes { get; set; } = [];
    public Expression<Func<T, object>>? OrderBy { get; set; }
    public bool IsDescending { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}
#endregion

#region Specs
public static class ItemSpec
{
    public static ISpecification<T3Item> All()
        => new InlineSpec<T3Item>(i => true)
        {
            Includes = [i => i.Module, i => i.Location!, i => i.ModuleType]
        };

    public static ISpecification<T3Item> ById(Guid id)
        => new InlineSpec<T3Item>(i => i.Id == id)
        {
            Includes = [i => i.ListParents, i => i.ListChilds]
        };

    public static ISpecification<T3Item> Tree()
        => new InlineSpec<T3Item>(i => true)
        {
            Includes = [i => i.ListParents, i => i.ListChilds]
        };

    public static ISpecification<T3Item> ByModule(Guid moduleId)
        => new InlineSpec<T3Item>(i => i.ModuleId == moduleId)
        {
            Includes = [i => i.Module]
        };

    public static ISpecification<T3Item> ByLocation(Guid locationId)
        => new InlineSpec<T3Item>(i => i.LocationId == locationId)
        {
            Includes = [i => i.Location!]
        };

    public static ISpecification<T3Item> ByName(string name)
        => new InlineSpec<T3Item>(i => i.Name == name);

    public static ISpecification<T3Item> Search(string keyword)
        => new InlineSpec<T3Item>(i =>
            i.Name.Contains(keyword) ||
            i.Barcode.Contains(keyword) ||
            i.SerialNumber.Contains(keyword))
        {
            OrderBy = i => i.Name
        };

    public static ISpecification<T3Item> WithHierarchy()
        => new InlineSpec<T3Item>(i => true)
        {
            Includes = [i => i.ListParents, i => i.ListChilds]
        };

    public static ISpecification<T3Item> Paged(string keyword, int skip, int take)
        => new InlineSpec<T3Item>(i =>
            i.Name.Contains(keyword))
        {
            OrderBy = i => i.Name,
            Skip = skip,
            Take = take
        };
}
public static class LocationSpec
{
    public static ISpecification<T3Location> All()
        => new InlineSpec<T3Location>(l => true)
        {
            Includes = [l => l.ListParents, l => l.ListChilds]
        };

    public static ISpecification<T3Location> ById(Guid id)
        => new InlineSpec<T3Location>(l => l.Id == id)
        {
            Includes = [l => l.ListItems, l => l.ListLocationItems]
        };

    public static ISpecification<T3Location> Tree()
        => new InlineSpec<T3Location>(l => true)
        {
            Includes = [l => l.ListParents, l => l.ListChilds]
        };

    public static ISpecification<T3Location> ByOperationNo(int operationNo)
        => new InlineSpec<T3Location>(l => l.OperationNo == operationNo);

    public static ISpecification<T3Location> Search(string keyword)
        => new InlineSpec<T3Location>(l =>
            l.Name.Contains(keyword) ||
            l.Barcode.Contains(keyword))
        {
            OrderBy = l => l.Name
        };

    public static ISpecification<T3Location> Paged(string keyword, int skip, int take)
        => new InlineSpec<T3Location>(l =>
            l.Name.Contains(keyword))
        {
            OrderBy = l => l.Name,
            Skip = skip,
            Take = take
        };
}
public static class ModuleSpec
{
    public static ISpecification<T3Module> All()
        => new InlineSpec<T3Module>(m => true)
        {
            Includes = [m => m.ListParents, m => m.ListChilds]
        };

    public static ISpecification<T3Module> ById(Guid id)
        => new InlineSpec<T3Module>(m => m.Id == id)
        {
            Includes = [m => m.ListItems]
        };

    public static ISpecification<T3Module> Tree()
        => new InlineSpec<T3Module>(m => true)
        {
            Includes = [m => m.ListParents, m => m.ListChilds]
        };

    public static ISpecification<T3Module> Search(string keyword)
        => new InlineSpec<T3Module>(m =>
            m.Name.Contains(keyword) ||
            m.PageText.Contains(keyword))
        {
            OrderBy = m => m.Name
        };

    public static ISpecification<T3Module> Paged(string keyword, int skip, int take)
        => new InlineSpec<T3Module>(m =>
            m.Name.Contains(keyword))
        {
            OrderBy = m => m.Name,
            Skip = skip,
            Take = take
        };
}
public static class UserSpec
{
    public static ISpecification<T3IdentityUser> All()
        => new InlineSpec<T3IdentityUser>(u => true)
        {
            Includes = [u => u.ListRoles, u => u.ListClaims]
        };

    public static ISpecification<T3IdentityUser> ById(Guid id)
        => new InlineSpec<T3IdentityUser>(u => u.Id == id)
        {
            Includes = [u => u.ListRoles, u => u.ListClaims]
        };

    public static ISpecification<T3IdentityUser> ByEmail(string email)
        => new InlineSpec<T3IdentityUser>(u => u.Email == email);

    public static ISpecification<T3IdentityUser> ByBarcode(string barcode)
        => new InlineSpec<T3IdentityUser>(u => u.Barcode == barcode);

    public static ISpecification<T3IdentityUser> ByUserId(string userId)
        => new InlineSpec<T3IdentityUser>(u => u.UserId == userId);

    public static ISpecification<T3IdentityUser> Search(string keyword)
        => new InlineSpec<T3IdentityUser>(u =>
            u.FirstName.Contains(keyword) ||
            u.LastName.Contains(keyword) ||
            u.Email.Contains(keyword))
        {
            OrderBy = u => u.FirstName
        };
}
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
public static class FormSpec
{
    public static ISpecification<T3Form> All()
        => new InlineSpec<T3Form>(f => true)
        {
            Includes = [f => f.CreateUser, f => f.ApprovedUser!, f => f.ListFormFields]
        };

    public static ISpecification<T3Form> ById(Guid id)
        => new InlineSpec<T3Form>(f => f.Id == id)
        {
            Includes = [f => f.ListFormFields]
        };

    public static ISpecification<T3Form> ByTemplate(Guid templateId)
        => new InlineSpec<T3Form>(f => f.TemplateId == templateId);

    public static ISpecification<T3Form> ByUser(Guid userId)
        => new InlineSpec<T3Form>(f => f.CreateUserId == userId);

    public static ISpecification<T3Form> Unapproved()
        => new InlineSpec<T3Form>(f => !f.IsApprove);

    public static ISpecification<T3Form> Paged(string keyword, int skip, int take)
        => new InlineSpec<T3Form>(f => f.ListFormFields.Any(x => x.PropertyField.Name.Contains(keyword)))
        {
            Skip = skip,
            Take = take
        };
}
public static class FormResourceSpec
{
    public static ISpecification<T3FormResource> All()
        => new InlineSpec<T3FormResource>(r => true);
}
public static class TemplateSpec
{
    public static ISpecification<T3Template> All()
        => new InlineSpec<T3Template>(t => true)
        {
            Includes = [t => t.ListForms, t => t.ListPanels, t => t.ListApprovers, t => t.ListPropertyFields]
        };

    public static ISpecification<T3Template> ById(Guid id)
        => new InlineSpec<T3Template>(t => t.Id == id)
        {
            Includes = [t => t.ListForms, t => t.ListApprovers]
        };

    public static ISpecification<T3Template> HasApprover(Guid userId)
        => new InlineSpec<T3Template>(t =>
            t.ListApprovers.Any(a => a.UserId == userId || a.RoleId == userId));
}
public static class PropertySpec
{
    public static ISpecification<T3Property> All()
        => new InlineSpec<T3Property>();

    public static ISpecification<T3Property> ByFormResource(Guid resourceId)
        => new InlineSpec<T3Property>(p => p.FormResourceId == resourceId);

    public static ISpecification<T3Property> IsRequired()
        => new InlineSpec<T3Property>(p => p.IsRequired);

    public static ISpecification<T3Property> Search(string keyword)
        => new InlineSpec<T3Property>(p =>
            p.Name.Contains(keyword) ||
            p.DisplayText.Contains(keyword))
        {
            OrderBy = p => p.Name
        };
}
public static class ProcessTypeSpec
{
    public static ISpecification<T3ProcessType> All()
        => new InlineSpec<T3ProcessType>();

    public static ISpecification<T3ProcessType> ById(Guid id)
        => new InlineSpec<T3ProcessType>(p => p.Id == id)
        {
            Includes = [p => p.ListItems, p => p.ListModules]
        };

    public static ISpecification<T3ProcessType> Search(string keyword)
        => new InlineSpec<T3ProcessType>(p =>
            p.Name.Contains(keyword) ||
            p.Barcode.Contains(keyword));
}
public static class ProtocolSpec
{
    public static ISpecification<T3Protocol> All()
        => new InlineSpec<T3Protocol>(p => true)
        {
            Includes = [p => p.ProcessType, p => p.ListProtocolItems]
        };

    public static ISpecification<T3Protocol> ByProcessType(Guid processTypeId)
        => new InlineSpec<T3Protocol>(p => p.ProcessTypeId == processTypeId);
}
public static class ShiftSpec
{
    public static ISpecification<T3Shift> All()
        => new InlineSpec<T3Shift>(s => true)
        {
            Includes = [s => s.Location, s => s.ListBreaks]
        };

    public static ISpecification<T3Shift> ByLocation(Guid locationId)
        => new InlineSpec<T3Shift>(s => s.LocationId == locationId);
}
public static class ShiftTypeSpec
{
    public static ISpecification<T3ShiftType> All()
        => new InlineSpec<T3ShiftType>(s => true)
        {
            Includes = [s => s.ListDays, s => s.ListLocations]
        };

    public static ISpecification<T3ShiftType> ById(Guid id)
        => new InlineSpec<T3ShiftType>(s => s.Id == id)
        {
            Includes = [s => s.ListDays]
        };
}
public static class ClaimSpec
{
    public static ISpecification<T3IdentityClaim> All()
        => new InlineSpec<T3IdentityClaim>(c => true)
        {
            Includes = [c => c.User!, c => c.Role!]
        };

    public static ISpecification<T3IdentityClaim> ByUserId(Guid userId)
        => new InlineSpec<T3IdentityClaim>(c => c.UserId == userId);

    public static ISpecification<T3IdentityClaim> ByRoleId(Guid roleId)
        => new InlineSpec<T3IdentityClaim>(c => c.RoleId == roleId);
}
#endregion