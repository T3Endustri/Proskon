using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Utilities;
using _02_Application.Interfaces;

namespace _02_Application.DependencyInjection;

public class AppSeeder(IUnitOfWork uow) : IAppSeeder
{
    public async Task SeedAsync()
    {
        // Rol: Admin
        var roleRepo = uow.Repository<T3IdentityRole>();
        var adminRole = (await roleRepo.WhereAsync(r => r.Name == "Admin")).FirstOrDefault();
        if (adminRole is null)
        {
            adminRole = new T3IdentityRole
            {
                Name = "Admin",
                StartPage = "/",
                IsActive = true,
                IsTeam = false,
                IsDepartment = false
            };
            await roleRepo.AddAsync(adminRole);
        }

        // Kullanıcı: admin
        var userRepo = uow.Repository<T3IdentityUser>();
        IReadOnlyList<T3IdentityUser> t3IdentityUsers = (await userRepo.WhereAsync(u => u.UserId == "admin"));
        var adminUser = t3IdentityUsers.FirstOrDefault();
        if (adminUser is null)
        {
            adminUser = new T3IdentityUser
            {
                UserId = "admin",
                FirstName = "System",
                LastName = "Admin",
                Email = "admin@example.com",
                PasswordHash = T3Helper.PasswordHasher.Hash("T3@Otomasyon2006"),
                Barcode = "ADMIN-001",
                StartPage = "/",
                IsActive = true,
                PhotoUrl = string.Empty
            };
            await userRepo.AddAsync(adminUser);
        }

        // İlişki: admin ↔ Admin
        var userRoleRepo = uow.Repository<T3IdentityUserRole>();
        var linked = await userRoleRepo.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
        if (!linked)
        {
            await userRoleRepo.AddAsync(new T3IdentityUserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
        }

        await uow.SaveChangesAsync();
    }
}