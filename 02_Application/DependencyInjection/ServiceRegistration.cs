using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using _01_Data.Context;
using _01_Data.Repositories;
using _02_Application.Interfaces;
using _02_Application.Services;

namespace _02_Application.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
    {
        // DbContext + UnitOfWork (Data katmanı DI)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

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
        services.AddScoped<IAppSeeder, AppSeeder>();

        return services;
    }
}