using _02_Application.DependencyInjection;
using _02_Application.Interfaces;
using _02_Application.Mappings;
using _02_Application.Services;
using ProskonUI.Components;
using ProskonUI.Services;
using ProskonUI.Services.Authorization;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzk4MzIwN0AzMzMwMmUzMDJlMzAzYjMzMzAzYmZLUGwvQ2J5eTl1Ums0dFJsZU41d1lSMHJWZU5kbHc2YTEvazQveTVBVVU9\r\n");

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

builder.Services.AddSyncfusionBlazor();
builder.Services.AddAutoMapper(typeof(ApplicationMappingProfile)); 
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ILogService, SerilogLogService>();
builder.Services.AddScoped<ToastService>();

builder.Services.AddApplicationServices(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);
 
var app = builder.Build();
 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStaticFiles(); // ✅ Bu gerekli

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
