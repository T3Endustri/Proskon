using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;
using Serilog;
using static _01_Data.Utilities.T3Helper;

namespace _02_Application.Services;

#region Identity Services
public class UserService(IUnitOfWork unitOfWork, IMapper mapper) : IUserService
{
    public async Task<UserListDto?> GetByIdAsync(Guid id)
    {
        var user = await unitOfWork.Repository<T3IdentityUser>().GetByIdAsync(id, u => u.ListRoles, u => u.ListClaims);
        return user is null ? null : mapper.Map<UserListDto>(user);
    }

    public async Task<List<UserListDto>> GetAllAsync()
    {
        var users = await unitOfWork.Repository<T3IdentityUser>().ListAsync(UserSpec.All());
        return mapper.Map<List<UserListDto>>(users);
    }

    public async Task AddAsync(UserDto dto)
    {
        var user = mapper.Map<T3IdentityUser>(dto);
        user.PasswordHash = PasswordHasher.Hash(dto.Password); // ✅ HASH!
        await unitOfWork.Repository<T3IdentityUser>().AddAsync(user);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserDto dto)
    {
        var user = await unitOfWork.Repository<T3IdentityUser>().GetByIdAsync(dto.Id)
                   ?? throw new Exception("Kullanıcı bulunamadı");

        // Şifre dışındaki bilgileri güncelle
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Email = dto.Email;
        user.PhotoUrl = dto.PhotoUrl;
        user.IsActive = dto.IsActive;

        await unitOfWork.Repository<T3IdentityUser>().UpdateAsync(user);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(UserChangePasswordDto dto)
    {
        var user = await unitOfWork.Repository<T3IdentityUser>().GetByIdAsync(dto.Id)
                   ?? throw new Exception("Kullanıcı bulunamadı");

        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword); // ✅ HASH!
        await unitOfWork.Repository<T3IdentityUser>().UpdateAsync(user);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3IdentityUser>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
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
            dto.Children = [.. role.ListChilds
                .Select(c => dict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))];
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
#endregion

#region Location Services
public class LocationService(IUnitOfWork unitOfWork, IMapper mapper) : ILocationService
{
    public async Task<List<LocationListDto>> GetAllAsync()
    {
        var locations = await unitOfWork.Repository<T3Location>().ListAsync(LocationSpec.All());
        return mapper.Map<List<LocationListDto>>(locations);
    }
    public async Task<LocationDto?> GetByIdAsync(Guid id)
    {
        var locations = await unitOfWork.Repository<T3Location>().ListAsync(LocationSpec.ById(id));
        return locations.Count == 0 ? null : mapper.Map<LocationDto>(locations[0]);
    }
    public async Task<List<LocationTreeDto>> GetTreeAsync()
    {
        var allLocations = await unitOfWork.Repository<T3Location>().ListAsync(LocationSpec.Tree());
        var dict = allLocations.ToDictionary(l => l.Id);
        var tree = new List<LocationTreeDto>();

        foreach (var loc in allLocations.Where(l => l.ListParents.Count == 0))
            tree.Add(BuildTree(loc, 0));

        LocationTreeDto BuildTree(T3Location location, int level)
        {
            var dto = mapper.Map<LocationTreeDto>(location);
            dto.Level = level;
            dto.Children = [.. location.ListChilds
                .Select(c => dict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))];
            return dto;
        }

        return tree;
    }
    public async Task<List<LocationHierarchyDto>> GetFlatHierarchyAsync(Guid locationId)
    {
        var location = await unitOfWork.Repository<T3Location>().GetByIdAsync(locationId, l => l.ListParents, l => l.ListChilds);
        if (location is null) return [];

        var hierarchies = location.ListParents.Concat(location.ListChilds).ToList();
        return mapper.Map<List<LocationHierarchyDto>>(hierarchies);
    }
    public async Task AddAsync(LocationDto dto)
    {
        var entity = mapper.Map<T3Location>(dto);
        await unitOfWork.Repository<T3Location>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(LocationDto dto)
    {
        var entity = mapper.Map<T3Location>(dto);
        await unitOfWork.Repository<T3Location>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Location>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3LocationHierarchy>();
        var exists = await repo.AnyAsync(h => h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3LocationHierarchy
            {
                Id = Guid.NewGuid(),
                ChildId = childId,
                ParentId = parentId
            };
            await repo.AddAsync(relation);
            await unitOfWork.SaveChangesAsync();
        }
    }
    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3LocationHierarchy>();
        var all = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var item in all)
            await repo.DeleteAsync(item.Id);

        await unitOfWork.SaveChangesAsync();
    }
}
#endregion

#region Form Services
public class FormService(IUnitOfWork unitOfWork, IMapper mapper) : IFormService
{
    public async Task<List<FormListDto>> GetAllAsync()
    {
        var spec = FormSpec.All();
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }
    public async Task<FormDto?> GetByIdAsync(Guid id)
    {
        var spec = FormSpec.ById(id);
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return forms.Count == 0 ? null : mapper.Map<FormDto>(forms[0]);
    }
    public async Task<List<FormListDto>> GetByUserAsync(Guid userId)
    {
        var spec = FormSpec.ByUser(userId);
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }
    public async Task<List<FormListDto>> GetByTemplateAsync(Guid templateId)
    {
        var spec = FormSpec.ByTemplate(templateId);
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }
    public async Task<List<FormListDto>> GetUnapprovedAsync()
    {
        var spec = FormSpec.Unapproved();
        var forms = await unitOfWork.Repository<T3Form>().ListAsync(spec);
        return mapper.Map<List<FormListDto>>(forms);
    }
    public async Task<(List<FormListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        var repo = unitOfWork.Repository<T3Form>();

        var (items, total) = await repo.PagingAsync(
            predicate: f => f.ListFormFields.Any(x => x.PropertyField.Name.Contains(keyword)),
            selector: f => mapper.Map<FormListDto>(f),
            orderBy: f => f.CreateTime,
            descending: true,
            skip,
            take
        );

        return (items, total);
    }
    public async Task AddAsync(FormDto dto)
    {
        var entity = mapper.Map<T3Form>(dto);
        await unitOfWork.Repository<T3Form>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(FormDto dto)
    {
        var entity = mapper.Map<T3Form>(dto);
        await unitOfWork.Repository<T3Form>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Form>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
public class FormResourceService(IUnitOfWork unitOfWork, IMapper mapper) : IFormResourceService
{
    public async Task<List<FormResourceListDto>> GetAllAsync()
    {
        var spec = FormResourceSpec.All();
        var resources = await unitOfWork.Repository<T3FormResource>().ListAsync(spec);
        return mapper.Map<List<FormResourceListDto>>(resources);
    }
    public async Task<FormResourceDto?> GetByIdAsync(Guid id)
    {
        var resource = await unitOfWork.Repository<T3FormResource>().GetByIdAsync(id, r => r.ListItems);
        return resource is null ? null : mapper.Map<FormResourceDto>(resource);
    }
    public async Task AddAsync(FormResourceDto dto)
    {
        var entity = mapper.Map<T3FormResource>(dto);
        await unitOfWork.Repository<T3FormResource>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(FormResourceDto dto)
    {
        var entity = mapper.Map<T3FormResource>(dto);
        await unitOfWork.Repository<T3FormResource>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3FormResource>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
#endregion

#region Template Services
public class TemplateService(IUnitOfWork unitOfWork, IMapper mapper) : ITemplateService
{
    public async Task<List<TemplateListDto>> GetAllAsync()
    {
        var templates = await unitOfWork.Repository<T3Template>().ListAsync(TemplateSpec.All());
        return mapper.Map<List<TemplateListDto>>(templates);
    }
    public async Task<TemplateDto?> GetByIdAsync(Guid id)
    {
        var templates = await unitOfWork.Repository<T3Template>().ListAsync(TemplateSpec.ById(id));
        return templates.Count == 0 ? null : mapper.Map<TemplateDto>(templates[0]);
    }
    public async Task<List<TemplateListDto>> GetByApproverAsync(Guid userId)
    {
        var templates = await unitOfWork.Repository<T3Template>().ListAsync(TemplateSpec.HasApprover(userId));
        return mapper.Map<List<TemplateListDto>>(templates);
    }
    public async Task AddAsync(TemplateDto dto)
    {
        var entity = mapper.Map<T3Template>(dto);
        await unitOfWork.Repository<T3Template>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(TemplateDto dto)
    {
        var entity = mapper.Map<T3Template>(dto);
        await unitOfWork.Repository<T3Template>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Template>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
public class PropertyService(IUnitOfWork unitOfWork, IMapper mapper) : IPropertyService
{
    public async Task<List<PropertyListDto>> GetAllAsync()
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.All());
        return mapper.Map<List<PropertyListDto>>(props);
    }
    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        var prop = await unitOfWork.Repository<T3Property>().GetByIdAsync(id, p => p.ListPanels, p => p.ListTemplates);
        return prop is null ? null : mapper.Map<PropertyDto>(prop);
    }
    public async Task<List<PropertyListDto>> SearchAsync(string keyword)
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.Search(keyword));
        return mapper.Map<List<PropertyListDto>>(props);
    }
    public async Task<List<PropertyListDto>> GetByFormResourceAsync(Guid resourceId)
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.ByFormResource(resourceId));
        return mapper.Map<List<PropertyListDto>>(props);
    }
    public async Task<List<PropertyDto>> GetRequiredAsync()
    {
        var props = await unitOfWork.Repository<T3Property>().ListAsync(PropertySpec.IsRequired());
        return mapper.Map<List<PropertyDto>>(props);
    }
    public async Task AddAsync(PropertyDto dto)
    {
        var entity = mapper.Map<T3Property>(dto);
        await unitOfWork.Repository<T3Property>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(PropertyDto dto)
    {
        var entity = mapper.Map<T3Property>(dto);
        await unitOfWork.Repository<T3Property>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Property>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
#endregion

#region Module and Item Services
public class ModuleService(IUnitOfWork unitOfWork, IMapper mapper) : IModuleService
{
    public async Task<List<ModuleListDto>> GetAllAsync()
    {
        var modules = await unitOfWork.Repository<T3Module>().ListAsync(ModuleSpec.All());
        return mapper.Map<List<ModuleListDto>>(modules);
    }
    public async Task<ModuleDto?> GetByIdAsync(Guid id)
    {
        var modules = await unitOfWork.Repository<T3Module>().ListAsync(ModuleSpec.ById(id));
        return modules.Count == 0 ? null : mapper.Map<ModuleDto>(modules[0]);
    }
    public async Task<List<ModuleTreeDto>> GetTreeAsync()
    {
        var allModules = await unitOfWork.Repository<T3Module>().ListAsync(ModuleSpec.Tree());
        var dict = allModules.ToDictionary(m => m.Id);
        var tree = new List<ModuleTreeDto>();

        foreach (var mod in allModules.Where(m => m.ListParents.Count == 0))
            tree.Add(BuildTree(mod, 0));

        ModuleTreeDto BuildTree(T3Module module, int level)
        {
            var dto = mapper.Map<ModuleTreeDto>(module);
            dto.Level = level;
            dto.Children = [.. module.ListChilds
                .Select(c => dict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))];
            return dto;
        }

        return tree;
    }
    public async Task<List<ModuleHierarchyDto>> GetFlatHierarchyAsync(Guid moduleId)
    {
        var module = await unitOfWork.Repository<T3Module>().GetByIdAsync(moduleId, m => m.ListParents, m => m.ListChilds);
        if (module is null) return [];

        var hierarchies = module.ListParents.Concat(module.ListChilds).ToList();
        return mapper.Map<List<ModuleHierarchyDto>>(hierarchies);
    }
    public async Task AddAsync(ModuleDto dto)
    {
        var entity = mapper.Map<T3Module>(dto);
        await unitOfWork.Repository<T3Module>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(ModuleDto dto)
    {
        var entity = mapper.Map<T3Module>(dto);
        await unitOfWork.Repository<T3Module>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Module>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3ModuleHierarchy>();
        var exists = await repo.AnyAsync(h => h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3ModuleHierarchy
            {
                Id = Guid.NewGuid(),
                ChildId = childId,
                ParentId = parentId
            };
            await repo.AddAsync(relation);
            await unitOfWork.SaveChangesAsync();
        }
    }
    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3ModuleHierarchy>();
        var all = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var item in all)
            await repo.DeleteAsync(item.Id);

        await unitOfWork.SaveChangesAsync();
    }
}
public class ItemService(IUnitOfWork unitOfWork, IMapper mapper) : IItemService
{
    public async Task<List<ItemListDto>> GetAllAsync()
    {
        var items = await unitOfWork.Repository<T3Item>().ListAsync(ItemSpec.All());
        return mapper.Map<List<ItemListDto>>(items);
    }
    public async Task<ItemDto?> GetByIdAsync(Guid id)
    {
        var items = await unitOfWork.Repository<T3Item>().ListAsync(ItemSpec.ById(id));
        return items.Count == 0 ? null : mapper.Map<ItemDto>(items[0]);
    }
    public async Task<List<ItemTreeDto>> GetTreeAsync()
    {
        var allItems = await unitOfWork.Repository<T3Item>().ListAsync(ItemSpec.Tree());
        var itemDict = allItems.ToDictionary(i => i.Id);
        var tree = new List<ItemTreeDto>();

        foreach (var item in allItems.Where(i => i.ListParents.Count == 0))
            tree.Add(BuildTree(item, 0));

        ItemTreeDto BuildTree(T3Item item, int level)
        {
            var dto = mapper.Map<ItemTreeDto>(item);
            dto.Level = level;
            dto.Children = [.. item.ListChilds
                .Select(c => itemDict[c.ChildId])
                .Select(c => BuildTree(c, level + 1))];
            return dto;
        }

        return tree;
    }
    public async Task<List<ItemHierarchyDto>> GetFlatHierarchyAsync(Guid itemId)
    {
        var item = await unitOfWork.Repository<T3Item>().GetByIdAsync(itemId, i => i.ListParents, i => i.ListChilds);
        if (item == null) return [];

        var hierarchies = item.ListParents.Concat(item.ListChilds).ToList();
        return mapper.Map<List<ItemHierarchyDto>>(hierarchies);
    }
    public async Task AddAsync(ItemDto dto)
    {
        var entity = mapper.Map<T3Item>(dto);
        await unitOfWork.Repository<T3Item>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(ItemDto dto)
    {
        var entity = mapper.Map<T3Item>(dto);
        await unitOfWork.Repository<T3Item>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Item>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3ItemHierarchy>();
        bool exists = await repo.AnyAsync(h => h.ChildId == childId && h.ParentId == parentId);
        if (!exists)
        {
            var relation = new T3ItemHierarchy
            {
                Id = Guid.NewGuid(),
                ChildId = childId,
                ParentId = parentId
            };
            await repo.AddAsync(relation);
            await unitOfWork.SaveChangesAsync();
        }
    }
    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        var repo = unitOfWork.Repository<T3ItemHierarchy>();
        var relations = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var relation in relations)
            await repo.DeleteAsync(relation.Id);

        await unitOfWork.SaveChangesAsync();
    }
}
#endregion

#region Process Type and Protocol Services
public class ProcessTypeService(IUnitOfWork unitOfWork, IMapper mapper) : IProcessTypeService
{
    public async Task<List<ProcessTypeListDto>> GetAllAsync()
    {
        var types = await unitOfWork.Repository<T3ProcessType>().ListAsync(ProcessTypeSpec.All());
        return mapper.Map<List<ProcessTypeListDto>>(types);
    }
    public async Task<ProcessTypeDto?> GetByIdAsync(Guid id)
    {
        var types = await unitOfWork.Repository<T3ProcessType>().ListAsync(ProcessTypeSpec.ById(id));
        return types.Count == 0 ? null : mapper.Map<ProcessTypeDto>(types[0]);
    }
    public async Task<List<ProcessTypeListDto>> SearchAsync(string keyword)
    {
        var types = await unitOfWork.Repository<T3ProcessType>().ListAsync(ProcessTypeSpec.Search(keyword));
        return mapper.Map<List<ProcessTypeListDto>>(types);
    }
    public async Task<(List<ProcessTypeListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        var repo = unitOfWork.Repository<T3ProcessType>();
        var (items, total) = await repo.PagingAsync(
            p => p.Name.Contains(keyword) || p.Barcode.Contains(keyword),
            selector: p => mapper.Map<ProcessTypeListDto>(p),
            orderBy: p => p.Name,
            descending: false,
            skip,
            take
        );
        return (items, total);
    }
    public async Task AddAsync(ProcessTypeDto dto)
    {
        var entity = mapper.Map<T3ProcessType>(dto);
        await unitOfWork.Repository<T3ProcessType>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(ProcessTypeDto dto)
    {
        var entity = mapper.Map<T3ProcessType>(dto);
        await unitOfWork.Repository<T3ProcessType>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3ProcessType>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task AssignItemAsync(Guid typeId, Guid itemId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeItem>();
        var exists = await repo.AnyAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        if (!exists)
        {
            await repo.AddAsync(new T3ProcessTypeItem
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ItemId = itemId
            });
            await unitOfWork.SaveChangesAsync();
        }
    }
    public async Task AssignModuleAsync(Guid typeId, Guid moduleId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeModule>();
        var exists = await repo.AnyAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        if (!exists)
        {
            await repo.AddAsync(new T3ProcessTypeModule
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ModuleId = moduleId
            });
            await unitOfWork.SaveChangesAsync();
        }
    }
    public async Task RemoveItemAsync(Guid typeId, Guid itemId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeItem>();
        var items = await repo.WhereAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        foreach (var i in items)
            await repo.DeleteAsync(i.Id);

        await unitOfWork.SaveChangesAsync();
    }
    public async Task RemoveModuleAsync(Guid typeId, Guid moduleId)
    {
        var repo = unitOfWork.Repository<T3ProcessTypeModule>();
        var modules = await repo.WhereAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        foreach (var m in modules)
            await repo.DeleteAsync(m.Id);

        await unitOfWork.SaveChangesAsync();
    }
}
public class ProtocolService(IUnitOfWork unitOfWork, IMapper mapper) : IProtocolService
{
    public async Task<List<ProtocolListDto>> GetAllAsync()
    {
        var protocols = await unitOfWork.Repository<T3Protocol>().ListAsync(ProtocolSpec.All());
        return mapper.Map<List<ProtocolListDto>>(protocols);
    }
    public async Task<ProtocolDto?> GetByIdAsync(Guid id)
    {
        var protocol = await unitOfWork.Repository<T3Protocol>().GetByIdAsync(id, p => p.ProcessType, p => p.ListProtocolItems);
        return protocol is null ? null : mapper.Map<ProtocolDto>(protocol);
    }
    public async Task<List<ProtocolListDto>> GetByProcessTypeAsync(Guid processTypeId)
    {
        var protocols = await unitOfWork.Repository<T3Protocol>().ListAsync(ProtocolSpec.ByProcessType(processTypeId));
        return mapper.Map<List<ProtocolListDto>>(protocols);
    }
    public async Task AddAsync(ProtocolDto dto)
    {
        var entity = mapper.Map<T3Protocol>(dto);
        await unitOfWork.Repository<T3Protocol>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(ProtocolDto dto)
    {
        var entity = mapper.Map<T3Protocol>(dto);
        await unitOfWork.Repository<T3Protocol>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Protocol>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
#endregion

#region Shift and Shift Type Services
public class ShiftTypeService(IUnitOfWork unitOfWork, IMapper mapper) : IShiftTypeService
{
    public async Task<List<ShiftTypeListDto>> GetAllAsync()
    {
        var types = await unitOfWork.Repository<T3ShiftType>().ListAsync(ShiftTypeSpec.All());
        return mapper.Map<List<ShiftTypeListDto>>(types);
    }

    public async Task<ShiftTypeDto?> GetByIdAsync(Guid id)
    {
        var types = await unitOfWork.Repository<T3ShiftType>().ListAsync(ShiftTypeSpec.ById(id));
        return types.Count == 0 ? null : mapper.Map<ShiftTypeDto>(types[0]);
    }

    public async Task AddAsync(ShiftTypeDto dto)
    {
        var entity = mapper.Map<T3ShiftType>(dto);
        await unitOfWork.Repository<T3ShiftType>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftTypeDto dto)
    {
        var entity = mapper.Map<T3ShiftType>(dto);
        await unitOfWork.Repository<T3ShiftType>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3ShiftType>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
public class ShiftService(IUnitOfWork unitOfWork, IMapper mapper) : IShiftService
{
    public async Task<List<ShiftListDto>> GetAllAsync()
    {
        var shifts = await unitOfWork.Repository<T3Shift>().ListAsync(ShiftSpec.All());
        return mapper.Map<List<ShiftListDto>>(shifts);
    }
    public async Task<ShiftDto?> GetByIdAsync(Guid id)
    {
        var shift = await unitOfWork.Repository<T3Shift>().GetByIdAsync(id, s => s.ListBreaks, s => s.Location);
        return shift is null ? null : mapper.Map<ShiftDto>(shift);
    }
    public async Task<List<ShiftListDto>> GetByLocationAsync(Guid locationId)
    {
        var shifts = await unitOfWork.Repository<T3Shift>().ListAsync(ShiftSpec.ByLocation(locationId));
        return mapper.Map<List<ShiftListDto>>(shifts);
    }
    public async Task AddAsync(ShiftDto dto)
    {
        var entity = mapper.Map<T3Shift>(dto);
        await unitOfWork.Repository<T3Shift>().AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task UpdateAsync(ShiftDto dto)
    {
        var entity = mapper.Map<T3Shift>(dto);
        await unitOfWork.Repository<T3Shift>().UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        await unitOfWork.Repository<T3Shift>().DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }
}
#endregion

public class SerilogLogService : ILogService
{
    public void Info(string source, string message)
    {
        Log.ForContext("Source", source).Information(message);
    }

    public void Warning(string source, string message)
    {
        Log.ForContext("Source", source).Warning(message);
    }

    public void Error(string source, string message, Exception? ex = null)
    {
        Log.ForContext("Source", source).Error(ex, message);
    }
}

public class AuthService(IUnitOfWork unitOfWork, IMapper mapper) : IAuthService
{
    public async Task<LoginResultDto> LoginAsync(LoginDto dto)
    {
        var spec = UserSpec.ByUserId(dto.UserId);
        var users = await unitOfWork.Repository<T3IdentityUser>().ListAsync(spec);
        var user = users.FirstOrDefault(u => u.IsActive);

        if (user is null)
            return new LoginResultDto { Success = false, Message = "Kullanıcı bulunamadı veya pasif" };

        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash))
            return new LoginResultDto { Success = false, Message = "Şifre hatalı" };

        return new LoginResultDto
        {
            Success = true,
            User = mapper.Map<UserListDto>(user)
        };
    }
}

