using _01_Data.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace _01_Data.Entities;

public class T3IdentityClaim : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? RoleId { get; set; }
    public short PermissionType { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public T3IdentityRole? Role { get; set; } = default!;
    public T3IdentityUser? User { get; set; } = default!;

}
public class T3IdentityRole : BaseEntity
{
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string StartPage { get; set; } = string.Empty;
    [Required] public bool IsActive { get; set; } = true;
    [Required] public bool IsTeam { get; set; } = false;
    [Required] public bool IsDepartment { get; set; } = false;
    public ICollection<T3IdentityRoleHierarchy> ListParents { get; set; } = [];
    public ICollection<T3IdentityRoleHierarchy> ListChilds { get; set; } = [];
    public ICollection<T3IdentityUserRole> ListUsers { get; set; } = [];
    public ICollection<T3IdentityClaim> ListClaims { get; set; } = [];
    public ICollection<T3TemplateApprover> ListApproveTemplates { get; set; } = [];

}
public class T3IdentityRoleHierarchy : BaseEntity
{
    [Required] public Guid ParentId { get; set; }
    [Required] public Guid ChildId { get; set; }
    public T3IdentityRole Parent { get; set; } = default!;
    public T3IdentityRole Child { get; set; } = default!;
}
public class T3IdentityUser : BaseEntity
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public string FirstName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string PasswordHash { get; set; } = string.Empty;
    [Required] public string Barcode { get; set; } = string.Empty;
    [Required] public string StartPage { get; set; } = string.Empty;
    [Required] public bool IsActive { get; set; } = true;
    [Required] public string PhotoUrl { get; set; } = string.Empty;
    public ICollection<T3Form> ListFormCreates { get; set; } = [];
    public ICollection<T3Form> ListFormApproveds { get; set; } = [];
    public ICollection<T3TemplateApprover> ListApproveTemplates { get; set; } = [];
    public ICollection<T3FormFieldValue> ListFormFieldValue { get; set; } = [];
    public ICollection<T3IdentityUserRole> ListRoles { get; set; } = [];
    public ICollection<T3IdentityClaim> ListClaims { get; set; } = [];
}
public class T3IdentityUserRole : BaseEntity
{
    [Required] public Guid UserId { get; set; } = default!;
    [Required] public Guid RoleId { get; set; } = default!;
    public T3IdentityUser User { get; set; } = default!;
    public T3IdentityRole Role { get; set; } = default!;
}