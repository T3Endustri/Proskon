using _01_Data.Context;
using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using static _01_Data.Utilities.T3Helper;

namespace _02_Application.Services;

#region Identity Services
public class UserService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IUserService
{
    public async Task<UserListDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();

        // Projection-first, sadece ihtiyaç olan alanlar
        var userDto = await uow.Repository<T3IdentityUser>()
            .Query()
            .Where(u => u.Id == id)
            .Select(u => new UserListDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                UserId = u.UserId,
                StartPage = u.StartPage,
                Roles = u.ListRoles.Select(ur => new RoleDto { Id = ur.RoleId, Name = ur.Role.Name }).ToList(),
                Claims = u.ListClaims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
            })
            .FirstOrDefaultAsync();

        return userDto;
    }

    public async Task<List<UserListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3IdentityUser>()
            .Query(UserSpec.All())
            .ProjectTo<UserListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(UserDto dto)
    {
        await using var uow = uowFactory.Create();
        var user = mapper.Map<T3IdentityUser>(dto);
        user.PasswordHash = PasswordHasher.Hash(dto.Password);
        await uow.Repository<T3IdentityUser>().AddAsync(user);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityUser>();

        var affected = await repo.Query()
            .Where(u => u.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.FirstName, dto.FirstName)
                .SetProperty(u => u.LastName, dto.LastName)
                .SetProperty(u => u.Email, dto.Email)
                .SetProperty(u => u.PhotoUrl, dto.PhotoUrl)
                .SetProperty(u => u.IsActive, dto.IsActive)
                .SetProperty(u => u.StartPage, dto.StartPage ?? string.Empty)
            );

        if (affected == 0)
            throw new Exception("Kullanıcı bulunamadı veya eşzamanlı değiştirildi.");
    }

    public async Task ChangePasswordAsync(UserChangePasswordDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityUser>();

        var newHash = PasswordHasher.Hash(dto.NewPassword);

        var affected = await repo.Query()
            .Where(u => u.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.PasswordHash, newHash)
            );

        if (affected == 0)
            throw new Exception("Kullanıcı bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3IdentityUser>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }

    public async Task<UserInfoDto> GetUserInfoAsync(Guid id, CancellationToken ct = default)
    {
        await using var uow = uowFactory.Create();

        var spec = UserSpec.ById(id);
        var user = await uow.Repository<T3IdentityUser>().FirstOrDefaultAsync(spec, ct);

        return user is null
            ? throw new KeyNotFoundException($"Kullanıcı bulunamadı (ID: {id})")
            : mapper.Map<UserInfoDto>(user);
    }
}
#endregion

#region Role Services
public class RoleService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IRoleService
{
    public async Task<List<RoleListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRole>();

        // 1) Temel alanlar (EF -> DTO)
        var items = await repo
            .Query(RoleSpec.All())
            .ProjectTo<RoleListDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        if (items.Count == 0) return items;

        // 2) Hiyerarşi grafiğini tek seferde çek
        var allRoles = await repo.ListAsync(RoleSpec.Tree()); // ListParents/ListChilds, Name, Id dolu
        var roleDict = allRoles.ToDictionary(r => r.Id);
        var (parentsMap, childrenMap) = BuildHierarchyMaps(allRoles);

        // 3) Kullanıcı adlarını roleId -> List<string> olarak çek (yalnızca ihtiyaç duyulan rollere)
        var userNamesByRole = await LoadUserNamesByRoleAsync(uow, [.. items.Select(i => i.Id)]);

        // 4) CSV’leri doldur
        var depthCache = new Dictionary<Guid, int>(); // kökten derinlik (min)
        foreach (var dto in items)
        {
            dto.ParentCsv = string.Join(", ",
                GetAncestorsOrdered(dto.Id, parentsMap, depthCache)
                .Select(id => roleDict.TryGetValue(id, out var e) ? e.Name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n)));

            dto.ChildCsv = string.Join(", ",
                GetDescendantsOrdered(dto.Id, childrenMap)
                .Select(id => roleDict.TryGetValue(id, out var e) ? e.Name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n)));

            if (userNamesByRole.TryGetValue(dto.Id, out var names) && names.Count > 0)
                dto.UserCsv = string.Join(", ", names.OrderBy(n => n));
            else
                dto.UserCsv = string.Empty;
        }

        return items;
    }
    public async Task<RoleDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3IdentityRole>()
            .Query(RoleSpec.ById(id))
            .ProjectTo<RoleDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RoleListDto>> SearchAsync(string keyword)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRole>();

        var items = await repo
            .Query(RoleSpec.SearchPaged(keyword, 0, int.MaxValue))
            .ProjectTo<RoleListDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        if (items.Count == 0) return items;

        var allRoles = await repo.ListAsync(RoleSpec.Tree());
        var roleDict = allRoles.ToDictionary(r => r.Id);
        var (parentsMap, childrenMap) = BuildHierarchyMaps(allRoles);

        var userNamesByRole = await LoadUserNamesByRoleAsync(uow, [.. items.Select(i => i.Id)]);

        var depthCache = new Dictionary<Guid, int>();
        foreach (var dto in items)
        {
            dto.ParentCsv = string.Join(", ",
                GetAncestorsOrdered(dto.Id, parentsMap, depthCache)
                .Select(id => roleDict.TryGetValue(id, out var e) ? e.Name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n)));

            dto.ChildCsv = string.Join(", ",
                GetDescendantsOrdered(dto.Id, childrenMap)
                .Select(id => roleDict.TryGetValue(id, out var e) ? e.Name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n)));

            if (userNamesByRole.TryGetValue(dto.Id, out var names) && names.Count > 0)
                dto.UserCsv = string.Join(", ", names.OrderBy(n => n));
            else
                dto.UserCsv = string.Empty;
        }

        return items;
    }

    public async Task<(List<RoleListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRole>();

        var baseQuery = repo.Query()
                            .Where(r => r.Name.Contains(keyword));

        var total = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderBy(r => r.Name)
            .Skip(skip)
            .Take(take)
            .ProjectTo<RoleListDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        if (items.Count == 0) return (items, total);

        var allRoles = await repo.ListAsync(RoleSpec.Tree());
        var roleDict = allRoles.ToDictionary(r => r.Id);
        var (parentsMap, childrenMap) = BuildHierarchyMaps(allRoles);

        // Sadece sayfadaki roller için kullanıcıları çek
        var pageRoleIds = items.Select(i => i.Id).ToList();
        var userNamesByRole = await LoadUserNamesByRoleAsync(uow, pageRoleIds);

        var depthCache = new Dictionary<Guid, int>();
        foreach (var dto in items)
        {
            dto.ParentCsv = string.Join(", ",
                GetAncestorsOrdered(dto.Id, parentsMap, depthCache)
                .Select(id => roleDict.TryGetValue(id, out var e) ? e.Name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n)));

            dto.ChildCsv = string.Join(", ",
                GetDescendantsOrdered(dto.Id, childrenMap)
                .Select(id => roleDict.TryGetValue(id, out var e) ? e.Name : null)
                .Where(n => !string.IsNullOrWhiteSpace(n)));

            if (userNamesByRole.TryGetValue(dto.Id, out var names) && names.Count > 0)
                dto.UserCsv = string.Join(", ", names.OrderBy(n => n));
            else
                dto.UserCsv = string.Empty;
        }

        return (items, total);
    }

    public async Task<List<RoleDto>> GetByUserIdAsync(Guid userId)
    {
        await using var uow = uowFactory.Create();
        var roles = await uow.Repository<T3IdentityRole>().ListAsync(RoleSpec.ByUserId(userId));
        return mapper.Map<List<RoleDto>>(roles);
    }

    public async Task<List<RoleTreeDto>> GetTreeAsync()
    {
        await using var uow = uowFactory.Create();
        var all = await uow.Repository<T3IdentityRole>().ListAsync(RoleSpec.All());
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
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3IdentityRole>(dto);
        await uow.Repository<T3IdentityRole>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(RoleDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRole>();

        var affected = await repo.Query()
            .Where(r => r.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Name, dto.Name.Trim())
                .SetProperty(r => r.StartPage, dto.StartPage ?? string.Empty)
                .SetProperty(r => r.IsActive, dto.IsActive)
                .SetProperty(r => r.IsTeam, dto.IsTeam)
                .SetProperty(r => r.IsDepartment, dto.IsDepartment)
            );

        if (affected == 0)
            throw new Exception("Rol bulunamadı, silinmiş olabilir veya eşzamanlı değiştirildi.");
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();

        // 1) Hiyerarşi ilişkilerini temizle (parent veya child olarak geçen tüm kayıtlar)
        var rhRepo = uow.Repository<T3IdentityRoleHierarchy>();
        await rhRepo.Query().Where(h => h.ParentId == id || h.ChildId == id).ExecuteDeleteAsync();

        // 2) Kullanıcı–rol bağlarını temizle (varsa)
        var urRepo = uow.Repository<T3IdentityUserRole>();
        await urRepo.Query().Where(ur => ur.RoleId == id).ExecuteDeleteAsync();

        // 3) (İsteğe bağlı) Role’e bağlı claim vb. başka tablolar varsa burada aynı şekilde temizle.

        // 4) Rolü sil
        await uow.Repository<T3IdentityRole>().DeleteAsync(id);

        // 5) Persist
        await uow.SaveChangesAsync();
    }


    public async Task<List<RoleDto>> GetAllChildRolesRecursiveAsync(Guid roleId)
    {
        await using var uow = uowFactory.Create();

        // Hiyerarşi için güvenli çekim: Tree spec
        var all = await uow.Repository<T3IdentityRole>().ListAsync(RoleSpec.Tree());

        var visited = new HashSet<Guid>();
        var resultIds = new List<Guid>();

        void Traverse(Guid id)
        {
            if (!visited.Add(id)) return;

            var current = all.FirstOrDefault(r => r.Id == id);
            if (current is null) return;

            foreach (var rel in current.ListChilds)
            {
                if (visited.Contains(rel.ChildId)) continue;
                resultIds.Add(rel.ChildId);
                Traverse(rel.ChildId);
            }
        }

        Traverse(roleId);

        // Id->Entity eşle, DTO’ya map et
        var dict = all.ToDictionary(r => r.Id);
        var resultEntities = resultIds.Where(dict.ContainsKey).Select(id => dict[id]).ToList();
        return mapper.Map<List<RoleDto>>(resultEntities);
    }

    public async Task<List<RoleDto>> GetAllParentRolesRecursiveAsync(Guid roleId)
    {
        await using var uow = uowFactory.Create();

        var all = await uow.Repository<T3IdentityRole>().ListAsync(RoleSpec.Tree());

        var visited = new HashSet<Guid>();
        var resultIds = new List<Guid>();

        void Traverse(Guid id)
        {
            if (!visited.Add(id)) return;

            var current = all.FirstOrDefault(r => r.Id == id);
            if (current is null) return;

            foreach (var rel in current.ListParents)
            {
                if (visited.Contains(rel.ParentId)) continue;
                resultIds.Add(rel.ParentId);
                Traverse(rel.ParentId);
            }
        }

        Traverse(roleId);

        var dict = all.ToDictionary(r => r.Id);
        var resultEntities = resultIds.Where(dict.ContainsKey).Select(id => dict[id]).ToList();
        return mapper.Map<List<RoleDto>>(resultEntities);
    }

    public async Task<IReadOnlyList<Guid>> GetDirectParentIdsAsync(Guid roleId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRoleHierarchy>();
        var ids = await repo.Query()
            .Where(h => h.ChildId == roleId)
            .Select(h => h.ParentId)
            .ToListAsync();
        return ids;
    }

    public async Task<IReadOnlyList<Guid>> GetDirectChildIdsAsync(Guid roleId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRoleHierarchy>();
        var ids = await repo.Query()
            .Where(h => h.ParentId == roleId)
            .Select(h => h.ChildId)
            .ToListAsync();
        return ids;
    }
    public async Task SetParentsAsync(Guid roleId, List<Guid> parentIds)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRoleHierarchy>();

        var desired = (parentIds ?? [])
            .Where(pid => pid != roleId)
            .Distinct()
            .ToList();

        // Mevcut ParentId'leri çek
        var existing = await repo.Query()
            .Where(h => h.ChildId == roleId)
            .Select(h => new { h.Id, h.ParentId })
            .ToListAsync();

        var existingParentIds = existing.Select(x => x.ParentId).ToHashSet();

        // Silinecekler → tek sorgu
        var toRemoveIds = existing.Where(x => !desired.Contains(x.ParentId))
                                  .Select(x => x.Id)
                                  .ToList();
        if (toRemoveIds.Count > 0)
            await repo.DeleteWhereAsync(h => toRemoveIds.Contains(h.Id));

        // Eklenecekler → toplu ekle
        var toAdd = desired.Where(pid => !existingParentIds.Contains(pid)).ToList();
        if (toAdd.Count > 0)
        {
            var links = toAdd.Select(pid => new T3IdentityRoleHierarchy
            {
                Id = Guid.NewGuid(),
                ParentId = pid,
                ChildId = roleId
            });
            await repo.AddRangeAsync(links);
        }

        await uow.SaveChangesAsync();
    }

    public async Task SetChildrenAsync(Guid roleId, List<Guid> childIds)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityRoleHierarchy>();

        var desired = (childIds ?? [])
            .Where(cid => cid != roleId)
            .Distinct()
            .ToList();

        // Mevcut ChildId'leri çek
        var existing = await repo.Query()
            .Where(h => h.ParentId == roleId)
            .Select(h => new { h.Id, h.ChildId })
            .ToListAsync();

        var existingChildIds = existing.Select(x => x.ChildId).ToHashSet();

        // Silinecekler → tek sorgu
        var toRemoveIds = existing.Where(x => !desired.Contains(x.ChildId))
                                  .Select(x => x.Id)
                                  .ToList();
        if (toRemoveIds.Count > 0)
            await repo.DeleteWhereAsync(h => toRemoveIds.Contains(h.Id));

        // Eklenecekler → toplu ekle
        var toAdd = desired.Where(cid => !existingChildIds.Contains(cid)).ToList();
        if (toAdd.Count > 0)
        {
            var links = toAdd.Select(cid => new T3IdentityRoleHierarchy
            {
                Id = Guid.NewGuid(),
                ParentId = roleId,
                ChildId = cid
            });
            await repo.AddRangeAsync(links);
        }

        await uow.SaveChangesAsync();
    }

    public async Task<RoleUsersDto> GetUsersAsync(Guid roleId)
    {
        await using var uow = uowFactory.Create();

        // Role bağlı kullanıcı Id'leri
        var userIds = await uow.Repository<T3IdentityUserRole>()
            .Query()
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync();

        if (userIds.Count == 0)
            return new RoleUsersDto(roleId, []);

        // Kullanıcıları MinimalUserDto olarak projekte et
        var users = await uow.Repository<T3IdentityUser>()
            .Query()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new MinimalUserDto(
                u.Id,
                (u.FirstName + " " + u.LastName).Trim(),
                u.Email
            ))
            .ToListAsync();

        return new RoleUsersDto(roleId, users);
    }

    public async Task AssignUsersAsync(AssignUsersToRoleRequest request)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityUserRole>();

        var requested = (request.UserIds ?? []).Distinct().ToList();

        // Mevcut kullanıcılar
        var existingUserIds = await repo.Query()
            .Where(ur => ur.RoleId == request.RoleId)
            .Select(ur => ur.UserId)
            .ToListAsync();

        // Silinecek ilişkiler
        var toRemove = existingUserIds.Except(requested).ToList();
        if (toRemove.Count > 0)
        {
            var rels = await repo.WhereAsync(ur => ur.RoleId == request.RoleId && toRemove.Contains(ur.UserId));
            foreach (var r in rels)
                await repo.DeleteAsync(r.Id);
        }

        // Eklenecek ilişkiler
        var toAdd = requested.Except(existingUserIds).ToList();
        foreach (var userId in toAdd)
        {
            await repo.AddAsync(new T3IdentityUserRole
            {
                Id = Guid.NewGuid(),
                RoleId = request.RoleId,
                UserId = userId
            });
        }

        await uow.SaveChangesAsync();
    }
    // Hiyerarşi grafını adjacency map'lere dönüştür
    private static (Dictionary<Guid, List<Guid>> ParentsMap, Dictionary<Guid, List<Guid>> ChildrenMap)
        BuildHierarchyMaps(IEnumerable<T3IdentityRole> roles)
    {
        var parentsMap = new Dictionary<Guid, List<Guid>>(); // childId -> [parentIds]
        var childrenMap = new Dictionary<Guid, List<Guid>>(); // parentId -> [childIds]

        foreach (var r in roles)
        {
            // Parent ilişkileri
            if (r.ListParents is { Count: > 0 })
            {
                var parents = r.ListParents.Select(p => p.ParentId).Distinct().ToList();
                if (parents.Count > 0)
                    parentsMap[r.Id] = parents;
            }

            // Child ilişkileri
            if (r.ListChilds is { Count: > 0 })
            {
                foreach (var c in r.ListChilds.Select(c => c.ChildId))
                {
                    if (!childrenMap.TryGetValue(r.Id, out var list))
                        childrenMap[r.Id] = list = [];
                    if (!list.Contains(c)) list.Add(c);
                }
            }
        }

        // Dict’lerde olmayanları da boş listeyle aç (lookup kolaylığı)
        foreach (var r in roles)
        {
            parentsMap.TryAdd(r.Id, []);
            childrenMap.TryAdd(r.Id, []);
        }

        return (parentsMap, childrenMap);
    }

    // Kökten uzaklık (min) memoization
    private static int GetDepthFromRoot(Guid id, Dictionary<Guid, List<Guid>> parentsMap, Dictionary<Guid, int> cache)
    {
        if (cache.TryGetValue(id, out var d)) return d;
        var parents = parentsMap.TryGetValue(id, out var list) ? list : [];
        if (parents.Count == 0)
        {
            cache[id] = 0;
            return 0;
        }

        var best = int.MaxValue;
        foreach (var p in parents)
        {
            var pd = GetDepthFromRoot(p, parentsMap, cache);
            if (pd + 1 < best) best = pd + 1;
        }
        cache[id] = best;
        return best;
    }

    // Ataları kökten başlayarak sırala (root->...->parent)
    private static IEnumerable<Guid> GetAncestorsOrdered(Guid id, Dictionary<Guid, List<Guid>> parentsMap, Dictionary<Guid, int> depthCache)
    {
        // Tüm ataları topla (DFS)
        var visited = new HashSet<Guid>();
        void Dfs(Guid cur)
        {
            if (!parentsMap.TryGetValue(cur, out var parents) || parents.Count == 0) return;
            foreach (var p in parents)
            {
                if (visited.Add(p))
                    Dfs(p);
            }
        }
        Dfs(id);

        // Kökten derinliğe göre sırala (küçük -> büyük)
        return [.. visited.OrderBy(a => GetDepthFromRoot(a, parentsMap, depthCache))];
    }

    // Tüm torunları rolden aşağı doğru sırala (yakın çocuklar önce)
    private static IEnumerable<Guid> GetDescendantsOrdered(Guid id, Dictionary<Guid, List<Guid>> childrenMap)
    {
        var ordered = new List<Guid>();
        var visited = new HashSet<Guid>();
        var q = new Queue<(Guid node, int depth)>();
        q.Enqueue((id, 0));

        while (q.Count > 0)
        {
            var (cur, d) = q.Dequeue();
            if (!childrenMap.TryGetValue(cur, out var children) || children.Count == 0) continue;

            foreach (var c in children)
            {
                if (visited.Add(c))
                {
                    ordered.Add(c);
                    q.Enqueue((c, d + 1));
                }
            }
        }

        // BFS zaten en yakınları önce getirir; istersen aynı derinlikte ada göre sıralayabilirsin
        return ordered;
    }

    // RoleId -> ["Ad Soyad", ...] sözlüğü (tek sorgu)
    private static async Task<Dictionary<Guid, List<string>>> LoadUserNamesByRoleAsync(IUnitOfWork uow, List<Guid> roleIds)
    {
        var urRepo = uow.Repository<T3IdentityUserRole>();
        var uRepo = uow.Repository<T3IdentityUser>();

        if (roleIds == null || roleIds.Count == 0)
            return [];

        var pairs = await urRepo.Query()
            .Where(ur => roleIds.Contains(ur.RoleId))
            .Join(
                uRepo.Query(),
                ur => ur.UserId,
                u => u.Id,
                (ur, u) => new { ur.RoleId, FullName = ((u.FirstName ?? string.Empty) + " " + (u.LastName ?? string.Empty)).Trim() }
            )
            .ToListAsync();

        var dict = new Dictionary<Guid, List<string>>();
        foreach (var g in pairs.GroupBy(x => x.RoleId))
        {
            dict[g.Key] = [.. g.Select(x => x.FullName)
                           .Where(n => !string.IsNullOrWhiteSpace(n))
                           .Distinct()];
        }
        return dict;
    }

}
#endregion

#region Claim Services
public class ClaimService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IClaimService
{
    public async Task<List<ClaimDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        var claims = await uow.Repository<T3IdentityClaim>().ListAsync(ClaimSpec.All());
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task<List<ClaimDto>> GetByUserIdAsync(Guid userId)
    {
        await using var uow = uowFactory.Create();
        var claims = await uow.Repository<T3IdentityClaim>().ListAsync(ClaimSpec.ByUserId(userId));
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task<List<ClaimDto>> GetByRoleIdAsync(Guid roleId)
    {
        await using var uow = uowFactory.Create();
        var claims = await uow.Repository<T3IdentityClaim>().ListAsync(ClaimSpec.ByRoleId(roleId));
        return mapper.Map<List<ClaimDto>>(claims);
    }

    public async Task AssignClaimsToUserAsync(ClaimAssignDto dto)
    {
        if (dto.UserId is null) return;
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityClaim>();

        var existing = await repo.WhereAsync(c => c.UserId == dto.UserId);
        foreach (var claim in existing)
            await repo.DeleteAsync(claim.Id);

        foreach (var n in dto.Claims)
        {
            var claim = new T3IdentityClaim
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                Type = n.Type,
                Value = n.Value,
                PermissionType = n.PermissionType
            };
            await repo.AddAsync(claim);
        }

        await uow.SaveChangesAsync();
    }

    public async Task AssignClaimsToRoleAsync(ClaimAssignDto dto)
    {
        if (dto.RoleId is null) return;
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3IdentityClaim>();

        var existing = await repo.WhereAsync(c => c.RoleId == dto.RoleId);
        foreach (var claim in existing)
            await repo.DeleteAsync(claim.Id);

        foreach (var n in dto.Claims)
        {
            var claim = new T3IdentityClaim
            {
                Id = Guid.NewGuid(),
                RoleId = dto.RoleId,
                Type = n.Type,
                Value = n.Value,
                PermissionType = n.PermissionType
            };
            await repo.AddAsync(claim);
        }

        await uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid claimId)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3IdentityClaim>().DeleteAsync(claimId);
        await uow.SaveChangesAsync();
    }
}
#endregion

#region Location Services
public class LocationService(IUnitOfWorkFactory uowFactory, IMapper mapper) : ILocationService
{
    public async Task<List<LocationListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Location>()
            .Query(LocationSpec.All())
            .ProjectTo<LocationListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<LocationDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Location>()
            .Query(LocationSpec.ById(id))
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<LocationTreeDto>> GetTreeAsync()
    {
        await using var uow = uowFactory.Create();
        var allLocations = await uow.Repository<T3Location>().ListAsync(LocationSpec.Tree());
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
        await using var uow = uowFactory.Create();
        var location = await uow.Repository<T3Location>()
                                .GetByIdAsync(locationId,
                                              asNoTracking: true,
                                              useIdentityResolution: true,      // opsiyonel
                                              l => l.ListParents, l => l.ListChilds);


        if (location is null) return [];

        var hierarchies = location.ListParents.Concat(location.ListChilds).ToList();
        return mapper.Map<List<LocationHierarchyDto>>(hierarchies);
    }

    public async Task AddAsync(LocationDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Location>(dto);
        await uow.Repository<T3Location>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(LocationDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Location>();

        var affected = await repo.Query()
            .Where(l => l.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(l => l.Name, dto.Name.Trim())
            );

        if (affected == 0)
            throw new Exception("Lokasyon bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Location>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3LocationHierarchy>();

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
            await uow.SaveChangesAsync();
        }
    }

    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3LocationHierarchy>();
        var all = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var item in all)
            await repo.DeleteAsync(item.Id);

        await uow.SaveChangesAsync();
    }
}
#endregion

#region Form Services
public class FormService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IFormService
{
    public async Task<List<FormListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Form>()
            .Query(FormSpec.All())
            .ProjectTo<FormListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<FormDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Form>()
            .Query(FormSpec.ById(id))
            .ProjectTo<FormDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<FormListDto>> GetByUserAsync(Guid userId)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Form>()
            .Query(FormSpec.ByUser(userId))
            .ProjectTo<FormListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<FormListDto>> GetByTemplateAsync(Guid templateId)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Form>()
            .Query(FormSpec.ByTemplate(templateId))
            .ProjectTo<FormListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<FormListDto>> GetUnapprovedAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Form>()
            .Query(FormSpec.Unapproved())
            .ProjectTo<FormListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // ProjectTo ile iki aşamalı paging (EF-translate edilebilir)
    public async Task<(List<FormListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Form>();

        var baseQuery = repo.Query()
                            .Where(f => f.ListFormFields.Any(x => x.PropertyField.Name.Contains(keyword)));

        var total = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderByDescending(f => f.CreateTime)
            .Skip(skip)
            .Take(take)
            .ProjectTo<FormListDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return (items, total);
    }

    public async Task AddAsync(FormDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Form>(dto);
        await uow.Repository<T3Form>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(FormDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Form>();

        var affected = await repo.Query()
            .Where(f => f.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(f => f.ApprovedUserId, dto.ApprovedUserId)
                .SetProperty(f => f.TemplateId, dto.TemplateId)
            );

        if (affected == 0)
            throw new Exception("Form bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Form>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }
}
#endregion

#region FormResource Services
public class FormResourceService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IFormResourceService
{
    public async Task<List<FormResourceListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3FormResource>()
            .Query(FormResourceSpec.All())
            .ProjectTo<FormResourceListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<FormResourceDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        var resource = await uow.Repository<T3FormResource>()
                                 .GetByIdAsync(id, r => r.ListItems);
        return resource is null ? null : mapper.Map<FormResourceDto>(resource);
    }

    public async Task AddAsync(FormResourceDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3FormResource>(dto);
        await uow.Repository<T3FormResource>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(FormResourceDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3FormResource>();

        var affected = await repo.Query()
            .Where(r => r.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Name, dto.Name.Trim())
            );

        if (affected == 0)
            throw new Exception("Form resource bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3FormResource>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }
}
#endregion

#region Template Services
public class TemplateService(IUnitOfWorkFactory uowFactory, IMapper mapper) : ITemplateService
{
    public async Task<List<TemplateListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Template>()
            .Query(TemplateSpec.All())
            .ProjectTo<TemplateListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<TemplateDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Template>()
            .Query(TemplateSpec.ById(id))
            .ProjectTo<TemplateDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TemplateListDto>> GetByApproverAsync(Guid userId)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Template>()
            .Query(TemplateSpec.HasApprover(userId))
            .ProjectTo<TemplateListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(TemplateDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Template>(dto);
        await uow.Repository<T3Template>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(TemplateDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Template>(dto);
        await uow.Repository<T3Template>().UpdateAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Template>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }
}
#endregion

#region Property Services
public class PropertyService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IPropertyService
{
    public async Task<List<PropertyListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Property>()
            .Query(PropertySpec.All())
            .ProjectTo<PropertyListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<PropertyDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        var prop = await uow.Repository<T3Property>()
                            .GetByIdAsync(id, true, p => p.ListPanels, p => p.ListTemplates);
        return prop is null ? null : mapper.Map<PropertyDto>(prop);
    }

    public async Task<List<PropertyListDto>> SearchAsync(string keyword)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Property>()
            .Query(PropertySpec.Search(keyword))
            .ProjectTo<PropertyListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<PropertyListDto>> GetByFormResourceAsync(Guid resourceId)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Property>()
            .Query(PropertySpec.ByFormResource(resourceId))
            .ProjectTo<PropertyListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<List<PropertyDto>> GetRequiredAsync()
    {
        await using var uow = uowFactory.Create();
        var props = await uow.Repository<T3Property>().ListAsync(PropertySpec.IsRequired());
        return mapper.Map<List<PropertyDto>>(props);
    }

    public async Task AddAsync(PropertyDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Property>(dto);
        await uow.Repository<T3Property>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(PropertyDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Property>();

        var affected = await repo.Query()
            .Where(p => p.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, dto.Name.Trim())
                .SetProperty(p => p.FormResourceId, dto.FormResourceId)
                .SetProperty(p => p.IsRequired, dto.IsRequired)
                .SetProperty(p => p.FileTypes, dto.FileTypes)
            );

        if (affected == 0)
            throw new Exception("Property bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Property>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }
}
#endregion

#region Module and Item Services
public class ModuleService(IUnitOfWorkFactory uowFactory,
                           IMapper mapper,
                           ICurrentUser current,
                           IMemoryCache cache,
                           IMenuCacheVersion version) : IModuleService
{
    public async Task<List<ModuleListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Module>()
            .Query(ModuleSpec.All())
            .ProjectTo<ModuleListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ModuleDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Module>()
            .Query(ModuleSpec.ById(id))
            .ProjectTo<ModuleDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ModuleTreeDto>> GetTreeAsync()
    {
        await using var uow = uowFactory.Create();
        var allModules = await uow.Repository<T3Module>().ListAsync(ModuleSpec.Tree());
        return BuildTreeDtos(allModules);
    }

    public async Task<List<ModuleHierarchyDto>> GetFlatHierarchyAsync(Guid moduleId)
    {
        await using var uow = uowFactory.Create();

        var module = await uow.Repository<T3Module>()
                              .GetByIdAsync(moduleId,
                                            asNoTracking: true,
                                            useIdentityResolution: true,      // opsiyonel açıklık
                                            m => m.ListParents, m => m.ListChilds);


        if (module is null) return [];

        var hierarchies = module.ListParents.Concat(module.ListChilds).ToList();
        return mapper.Map<List<ModuleHierarchyDto>>(hierarchies);
    }

    public async Task AddAsync(ModuleDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Module>(dto);
        await uow.Repository<T3Module>().AddAsync(entity);
        await uow.SaveChangesAsync();
        InvalidateAdminMenuCache();
    }

    public async Task UpdateAsync(ModuleDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Module>();

        var affected = await repo.Query()
            .Where(m => m.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Name, dto.Name.Trim())
                .SetProperty(m => m.PageText, dto.PageText)
                .SetProperty(m => m.IsCanBarcode, dto.IsCanBarcode)
                .SetProperty(m => m.IsCanPage, dto.IsCanPage)
                .SetProperty(m => m.IsCanTemplate, dto.IsCanTemplate)
                .SetProperty(m => m.IsCanModuleType, dto.IsCanModuleType)
                .SetProperty(m => m.IsCanTarget, dto.IsCanTarget)
                .SetProperty(m => m.ColorBack, dto.ColorBack)
                .SetProperty(m => m.ColorFore, dto.ColorFore)
                .SetProperty(m => m.IconMultiple, dto.IconMultiple)
                .SetProperty(m => m.IconSingle, dto.IconSingle)
                .SetProperty(m => m.SortBy, dto.SortBy)
            );

        if (affected == 0)
            throw new Exception("Modül bulunamadı veya eşzamanlı değiştirildi.");

        InvalidateAdminMenuCache();
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Module>().DeleteAsync(id);
        await uow.SaveChangesAsync();
        InvalidateAdminMenuCache();
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ModuleHierarchy>();
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
            await uow.SaveChangesAsync();
            InvalidateAdminMenuCache();
        }
    }

    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ModuleHierarchy>();
        var all = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var item in all)
            await repo.DeleteAsync(item.Id);

        await uow.SaveChangesAsync();
        InvalidateAdminMenuCache();
    }

    public async Task<List<ModuleTreeDto>> GetTreeForCurrentAsync()
    {
        await current.EnsureLoadedAsync();
        if (!current.IsAuthenticated) return [];

        var key = current.HasRole("Admin")
            ? $"menu:v{version.Version}:admin"
            : $"menu:v{version.Version}:user:{current.UserId}";

        var result = await cache.GetOrCreateAsync<List<ModuleTreeDto>>(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(45);

            await using var uow = uowFactory.Create();
            var allModules = await uow.Repository<T3Module>().ListAsync(ModuleSpec.Tree());
            var fullTree = BuildTreeDtos(allModules);

            if (current.HasRole("Admin"))
                return fullTree;

            var claimValues = current.Claims
                                     .Select(c => c.Value)
                                     .Where(v => !string.IsNullOrWhiteSpace(v))
                                     .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return FilterDtoTree(fullTree, claimValues);
        });

        return result ?? [];
    }

    private List<ModuleTreeDto> BuildTreeDtos(List<T3Module> allModules)
    {
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

    private static List<ModuleTreeDto> FilterDtoTree(List<ModuleTreeDto> nodes, HashSet<string> claimValues)
    {
        var result = new List<ModuleTreeDto>();

        foreach (var n in nodes)
        {
            var filteredChildren = FilterDtoTree(n.Children ?? [], claimValues);
            var allowedHere = n.IsCanPage && claimValues.Contains(n.Id.ToString());

            if (allowedHere || filteredChildren.Count > 0)
            {
                result.Add(new ModuleTreeDto
                {
                    Id = n.Id,
                    Name = n.Name,
                    PageText = n.PageText,
                    ColorBack = n.ColorBack,
                    ColorFore = n.ColorFore,
                    IconMultiple = n.IconMultiple,
                    IsCanPage = n.IsCanPage,
                    Level = n.Level,
                    Children = filteredChildren
                });
            }
        }

        return result;
    }

    public void InvalidateMenuCache(Guid userId)
        => cache.Remove($"menu:v{version.Version}:user:{userId}");

    public void InvalidateAdminMenuCache()
        => version.Bump();
}
#endregion

#region Item Services
public class ItemService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IItemService
{
    public async Task<List<ItemListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Item>()
            .Query(ItemSpec.All())
            .ProjectTo<ItemListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ItemDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Item>()
            .Query(ItemSpec.ById(id))
            .ProjectTo<ItemDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ItemTreeDto>> GetTreeAsync()
    {
        await using var uow = uowFactory.Create();
        var allItems = await uow.Repository<T3Item>().ListAsync(ItemSpec.Tree());
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
        await using var uow = uowFactory.Create();
        var item = await uow.Repository<T3Item>()
                            .GetByIdAsync(itemId,
                                          asNoTracking: true,
                                          useIdentityResolution: true,      // opsiyonel
                                          i => i.ListParents, i => i.ListChilds);

        if (item is null) return [];

        var hierarchies = item.ListParents.Concat(item.ListChilds).ToList();
        return mapper.Map<List<ItemHierarchyDto>>(hierarchies);
    }


    public async Task AddAsync(ItemDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Item>(dto);
        await uow.Repository<T3Item>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(ItemDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Item>();

        var affected = await repo.Query()
            .Where(i => i.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(i => i.Name, dto.Name.Trim())
                .SetProperty(i => i.ModuleId, dto.ModuleId)
                .SetProperty(i => i.LocationId, dto.LocationId)
                .SetProperty(i => i.ModuleTypeId, dto.ModuleTypeId)
            );

        if (affected == 0)
            throw new Exception("Item bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Item>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }

    public async Task AssignParentAsync(Guid childId, Guid parentId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ItemHierarchy>();
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
            await uow.SaveChangesAsync();
        }
    }

    public async Task RemoveParentAsync(Guid childId, Guid parentId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ItemHierarchy>();
        var relations = await repo.WhereAsync(h => h.ChildId == childId && h.ParentId == parentId);
        foreach (var relation in relations)
            await repo.DeleteAsync(relation.Id);

        await uow.SaveChangesAsync();
    }
}
#endregion

#region Process Type and Protocol Services
public class ProcessTypeService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IProcessTypeService
{
    public async Task<List<ProcessTypeListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3ProcessType>()
            .Query(ProcessTypeSpec.All())
            .ProjectTo<ProcessTypeListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ProcessTypeDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3ProcessType>()
            .Query(ProcessTypeSpec.ById(id))
            .ProjectTo<ProcessTypeDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ProcessTypeListDto>> SearchAsync(string keyword)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3ProcessType>()
            .Query(ProcessTypeSpec.Search(keyword))
            .ProjectTo<ProcessTypeListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    // ProjectTo ile paging (mapper.Map selector yerine)
    public async Task<(List<ProcessTypeListDto> Items, int TotalCount)> GetPagedAsync(string keyword, int skip, int take)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ProcessType>();

        var baseQuery = repo.Query()
                            .Where(p => p.Name.Contains(keyword) || p.Barcode.Contains(keyword));

        var total = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderBy(p => p.Name)
            .Skip(skip)
            .Take(take)
            .ProjectTo<ProcessTypeListDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return (items, total);
    }

    public async Task AddAsync(ProcessTypeDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3ProcessType>(dto);
        await uow.Repository<T3ProcessType>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProcessTypeDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ProcessType>();

        var affected = await repo.Query()
            .Where(p => p.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, dto.Name.Trim())
                .SetProperty(p => p.Barcode, dto.Barcode)
            );

        if (affected == 0)
            throw new Exception("ProcessType bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3ProcessType>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }

    public async Task AssignItemAsync(Guid typeId, Guid itemId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ProcessTypeItem>();
        var exists = await repo.AnyAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        if (!exists)
        {
            await repo.AddAsync(new T3ProcessTypeItem
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ItemId = itemId
            });
            await uow.SaveChangesAsync();
        }
    }

    public async Task AssignModuleAsync(Guid typeId, Guid moduleId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ProcessTypeModule>();
        var exists = await repo.AnyAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        if (!exists)
        {
            await repo.AddAsync(new T3ProcessTypeModule
            {
                Id = Guid.NewGuid(),
                TypeId = typeId,
                ModuleId = moduleId
            });
            await uow.SaveChangesAsync();
        }
    }

    public async Task RemoveItemAsync(Guid typeId, Guid itemId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ProcessTypeItem>();
        var items = await repo.WhereAsync(x => x.TypeId == typeId && x.ItemId == itemId);
        foreach (var i in items) await repo.DeleteAsync(i.Id);
        await uow.SaveChangesAsync();
    }

    public async Task RemoveModuleAsync(Guid typeId, Guid moduleId)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ProcessTypeModule>();
        var modules = await repo.WhereAsync(x => x.TypeId == typeId && x.ModuleId == moduleId);
        foreach (var m in modules) await repo.DeleteAsync(m.Id);
        await uow.SaveChangesAsync();
    }
}

public class ProtocolService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IProtocolService
{
    public async Task<List<ProtocolListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Protocol>()
            .Query(ProtocolSpec.All())
            .ProjectTo<ProtocolListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ProtocolDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        var protocol = await uow.Repository<T3Protocol>()
                                 .GetByIdAsync(id, true, p => p.ProcessType, p => p.ListProtocolItems);
        return protocol is null ? null : mapper.Map<ProtocolDto>(protocol);
    }

    public async Task<List<ProtocolListDto>> GetByProcessTypeAsync(Guid processTypeId)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Protocol>()
            .Query(ProtocolSpec.ByProcessType(processTypeId))
            .ProjectTo<ProtocolListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(ProtocolDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Protocol>(dto);
        await uow.Repository<T3Protocol>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(ProtocolDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Protocol>();

        var affected = await repo.Query()
            .Where(p => p.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Name, dto.Name.Trim())
                .SetProperty(p => p.ProcessTypeId, dto.ProcessTypeId)
            );

        if (affected == 0)
            throw new Exception("Protokol bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Protocol>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }
}
#endregion

#region Shift and Shift Type Services
public class ShiftTypeService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IShiftTypeService
{
    public async Task<List<ShiftTypeListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3ShiftType>()
            .Query(ShiftTypeSpec.All())
            .ProjectTo<ShiftTypeListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ShiftTypeDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3ShiftType>()
            .Query(ShiftTypeSpec.ById(id))
            .ProjectTo<ShiftTypeDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(ShiftTypeDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3ShiftType>(dto);
        await uow.Repository<T3ShiftType>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftTypeDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3ShiftType>();

        var affected = await repo.Query()
            .Where(st => st.Id == dto.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(st => st.Name, dto.Name.Trim())
            );

        if (affected == 0)
            throw new Exception("Vardiya tipi bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3ShiftType>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }
}

public class ShiftService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IShiftService
{
    public async Task<List<ShiftListDto>> GetAllAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Shift>()
            .Query(ShiftSpec.All())
            .ProjectTo<ShiftListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ShiftDto?> GetByIdAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        var shift = await uow.Repository<T3Shift>()
                             .GetByIdAsync(id, true, s => s.ListBreaks, s => s.Location);
        return shift is null ? null : mapper.Map<ShiftDto>(shift);
    }

    public async Task<List<ShiftListDto>> GetByLocationAsync(Guid locationId)
    {
        await using var uow = uowFactory.Create();
        return await uow.Repository<T3Shift>()
            .Query(ShiftSpec.ByLocation(locationId))
            .ProjectTo<ShiftListDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(ShiftDto dto)
    {
        await using var uow = uowFactory.Create();
        var entity = mapper.Map<T3Shift>(dto);
        await uow.Repository<T3Shift>().AddAsync(entity);
        await uow.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShiftDto dto)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Shift>();

        var affected = await repo.Query()
            .Where(s => s.Id == dto.Id)
            .ExecuteUpdateAsync(su => su
                .SetProperty(s => s.Name, dto.Name.Trim())
                .SetProperty(s => s.LocationId, dto.LocationId)
            );

        if (affected == 0)
            throw new Exception("Vardiya bulunamadı veya eşzamanlı değiştirildi.");
    }


    public async Task DeleteAsync(Guid id)
    {
        await using var uow = uowFactory.Create();
        await uow.Repository<T3Shift>().DeleteAsync(id);
        await uow.SaveChangesAsync();
    }
}
#endregion

#region Authentication and Current User Services
public class AuthService(IUnitOfWorkFactory uowFactory, IMapper mapper) : IAuthService
{
    public async Task<LoginResultDto> LoginAsync(LoginDto loginDto)
    {
        await using var uow = uowFactory.Create();

        var users = await uow.Repository<T3IdentityUser>().ListAsync(UserSpec.ByUserId(loginDto.UserId));
        var user = users.FirstOrDefault(u => u.IsActive);
        if (user is null)
            return new LoginResultDto { Success = false, Message = "Kullanıcı bulunamadı veya pasif" };

        if (!PasswordHasher.Verify(loginDto.Password, user.PasswordHash))
            return new LoginResultDto { Success = false, Message = "Şifre hatalı" };

        return new LoginResultDto { Success = true, User = mapper.Map<UserListDto>(user) };
    }
}
#endregion

#region Logging
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
#endregion

#region HealthCheck & Seeding
// HealthCheck singleton’ına scoped bağımlılık verilmemesi için fabrikayla scope açıyoruz.
public sealed class DatabaseHealthCheck(IUnitOfWorkFactory uowFactory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var uow = uowFactory.Create();
            var ok = await uow.CanConnectAsync(cancellationToken);
            return ok
                ? HealthCheckResult.Healthy("Database reachable")
                : HealthCheckResult.Unhealthy("Database not reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database check failed", ex);
        }
    }
}

public sealed class SeedService(IDbContextFactory<ApplicationDbContext> factory,
                                IUnitOfWorkFactory uowFactory,
                                IMapper mapper) : ISeedService
{
    public async Task EnsureSeedAsync()
    {
        // 1) Migrate
        await using (var db = await factory.CreateDbContextAsync())
            await db.Database.MigrateAsync();

        await EnsureAdminRoleAndUserAsync();

        // Modül tanımları
        var modules = new List<ModuleDto>
        {
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "ProductCategory",
                PageText = "Ürün Kategorileri",
                IsCanBarcode = true,
                IsCanPage = true,
                IsCanTemplate = true,
                IsCanTarget = true,
                ColorBack = "#0047AB",
                ColorFore = "#FFFFFF",
                IconMultiple = (int)IconName.bi_cc_circle_fill,
                IconSingle = (int)IconName.bi_cc_circle,
                SortBy = 10,
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Product",
                PageText = "Ürünler",
                IsCanBarcode = true,
                IsCanPage = false,
                IsCanTemplate = true,
                IsCanTarget = true,
                ColorBack = "#305CDE",
                ColorFore = "#FFFFFF",
                IconMultiple = (int)IconName.bi_p_circle_fill,
                IconSingle = (int)IconName.bi_p_circle_fill,
                SortBy = 20,
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test Kategori",
                PageText = "Test Kategorileri",
                IsCanBarcode = true,
                IsCanPage = true,
                IsCanTemplate = true,
                IsCanModuleType = true,
                IsCanTarget = true,
                ColorBack = "#520075",
                ColorFore = "#FFFFFF",
                IconMultiple = (int)IconName.bi_sign_intersection_t_fill,
                IconSingle = (int)IconName.bi_sign_intersection_t,
                SortBy = 30,
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                PageText = "Testler",
                IsCanBarcode = true,
                IsCanPage = false,
                IsCanTemplate = false,
                IsCanTarget = true,
                ColorBack = "#8A00C4",
                ColorFore = "#FFFFFF",
                IconMultiple = (int)IconName.bi_sign_intersection_t_fill,
                IconSingle = (int)IconName.bi_sign_intersection_t,
                SortBy = 40,
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Duruş Kategori",
                PageText = "Duruş Kategorileri",
                IsCanBarcode = true,
                IsCanPage = true,
                IsCanTemplate = true,
                ColorBack = "#E73121",
                ColorFore = "#FFFFFF",
                IconMultiple = (int)IconName.bi_pause_circle_fill,
                IconSingle = (int)IconName.bi_pause_circle,
                SortBy = 50,
            },
            new ()
            {
                Id = Guid.NewGuid(),
                Name = "Uygunsuzluk Kategori",
                PageText = "Uygunsuzluk Kategorileri",
                IsCanBarcode = true,
                IsCanPage = true,
                IsCanTemplate = true,
                ColorBack = "#CC7722",
                ColorFore = "#FFFFFF",
                IconMultiple = (int)IconName.bi_exclamation_triangle_fill,
                IconSingle = (int)IconName.bi_exclamation_triangle,
                SortBy = 60,
            },
        };

        await EnsureModulesAsync(modules, mapper);
    }

    private async Task EnsureAdminRoleAndUserAsync()
    {
        await using var uow = uowFactory.Create();

        var roleRepo = uow.Repository<T3IdentityRole>();
        var admin = await roleRepo.FirstOrDefaultAsync(r => r.Name == "Admin");

        if (admin is null)
        {
            admin = new T3IdentityRole
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                IsActive = true,
                StartPage = "/manage"
            };
            await roleRepo.AddAsync(admin);
            await uow.SaveChangesAsync();
        }

        var userRepo = uow.Repository<T3IdentityUser>();
        var userRoleRepo = uow.Repository<T3IdentityUserRole>();

        var t3 = await userRepo.FirstOrDefaultAsync(r => r.UserId == "t3");

        if (t3 is null)
        {
            t3 = new T3IdentityUser
            {
                Id = Guid.NewGuid(),
                UserId = "t3",
                FirstName = "T3",
                LastName = "User",
                Email = "t3@local",
                IsActive = true,
                StartPage = "/manage",
                PasswordHash = PasswordHasher.Hash("T3@1234")
            };

            await userRepo.AddAsync(t3);
            await userRoleRepo.AddAsync(new T3IdentityUserRole
            {
                Id = Guid.NewGuid(),
                UserId = t3.Id,
                RoleId = admin.Id
            });

            await uow.SaveChangesAsync();
        }
        else
        {
            bool hasAdmin = (await userRoleRepo.WhereAsync(x => x.UserId == t3.Id && x.RoleId == admin.Id)).Any();
            if (!hasAdmin)
            {
                await userRoleRepo.AddAsync(new T3IdentityUserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = t3.Id,
                    RoleId = admin.Id
                });
                await uow.SaveChangesAsync();
            }
        }
    }

    private async Task EnsureModulesAsync(IEnumerable<ModuleDto> modules, IMapper mapper)
    {
        foreach (var module in modules)
        {
            await EnsureModuleAsync(module, mapper);
        }
    }

    private async Task EnsureModuleAsync(ModuleDto dto, IMapper mapper)
    {
        await using var uow = uowFactory.Create();
        var repo = uow.Repository<T3Module>();

        var existing = await repo.FirstOrDefaultAsync(r => r.Name == dto.Name);

        if (existing is null)
        {
            var entity = mapper.Map<T3Module>(dto);
            entity.Id = Guid.NewGuid();
            await repo.AddAsync(entity);
        }
        else
        {
            mapper.Map(dto, existing);
            await repo.UpdateAsync(existing);
        }

        await uow.SaveChangesAsync();
    }
}
#endregion

#region DI Registration
public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
    {
        services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
                sql.CommandTimeout(60);
                sql.MaxBatchSize(100);
                sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            // Zaten global NoTracking kullanıyoruz
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

#if !DEBUG
    options.EnableThreadSafetyChecks(false);
#endif
        }, poolSize: 128);


        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

        // Caching & versioning
        services.AddMemoryCache();
        services.AddSingleton<IMenuCacheVersion, MenuCacheVersion>();

        // Logging
        services.AddSingleton<ILogService, SerilogLogService>();

        // HealthChecks (UI katmanı sadece endpoint'i map'ler)
        services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("db");

        // Application Servisleri (UI sadece bunlarla çalışır)
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<IFormService, FormService>();
        services.AddScoped<IFormResourceService, FormResourceService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IProcessTypeService, ProcessTypeService>();
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<IProtocolService, ProtocolService>();
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IShiftTypeService, ShiftTypeService>();
        services.AddScoped<ITemplateService, TemplateService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISeedService, SeedService>();

        return services;
    }
}
#endregion

public sealed class MenuCacheVersion : IMenuCacheVersion
{
    private long _v;
    public long Version => Interlocked.Read(ref _v);
    public void Bump() => Interlocked.Increment(ref _v);
}
