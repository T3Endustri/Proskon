
using _02_Application.Interfaces;
using _02_Application.Mappings;
using _02_Application.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;

using ProskonUI.Components;
using ProskonUI.Hubs;
using ProskonUI.Services.Authorization;
using ProskonUI.Services.Middlewares;
using ProskonUI.Services.Toasts;
using Serilog;
using Serilog.Context;
using Syncfusion.Blazor;
using System.IO.Compression;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Syncfusion lisansı
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzk4MzIwN0AzMzMwMmUzMDJlMzAzYjMzMzAzYmZLUGwvQ2J5eTl1Ums0dFJsZU41d1lSMHJWZU5kbHc2YTEvazQveTVBVVU9\r\n");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning) // Info altını console’a yazma
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();


builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents(options =>
                {
                    options.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(30);
                    options.MaxBufferedUnacknowledgedRenderBatches = 10; // server belleğini korur
                });

builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(
                    Path.Combine(builder.Environment.ContentRootPath, "dpkeys")))  
                .SetApplicationName("Proskon")             
                .SetDefaultKeyLifetime(TimeSpan.FromDays(180));

builder.Services.AddSignalR();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"]; 
    cfg.AddProfile<ApplicationMappingProfile>();
}, typeof(ApplicationMappingProfile).Assembly);

builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
builder.Services.AddScoped<IT3AuthService, T3AuthService>();
builder.Services.AddScoped<ILogService, SerilogLogService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddSingleton<IMenuCacheVersion, MenuCacheVersion>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Host.UseSerilog();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();

    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
    [
        "application/json",
        "application/octet-stream",
        "application/wasm",
        "text/plain",
        "text/css",
        "application/javascript",
        "image/svg+xml"
    ]);
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.AddApplicationServices(
    builder.Configuration.GetConnectionString("DefaultConnection")!
);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".Proskon.Auth";   // sabit ve benzersiz ad
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/denied";
        options.ReturnUrlParameter = "returnUrl";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.ClaimsIssuer = "Proskon";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RoleManagement", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.IsInRole("Admin") ||
            ctx.User.IsInRole("Manager") ||
            ctx.User.HasClaim(c =>
                (c.Type.Equals("permission", StringComparison.OrdinalIgnoreCase) ||
                 c.Type.Equals("Permissions", StringComparison.OrdinalIgnoreCase)) &&
                 c.Value.Equals("roles.manage", StringComparison.OrdinalIgnoreCase))
        )
)
    .AddPolicy("ItemRead", policy =>
        policy.RequireAssertion(ctx =>
            // global okuma
            ctx.User.HasClaim(c =>
                c.Type.Equals("access", StringComparison.OrdinalIgnoreCase) &&
                c.Value.Equals("global:item:read", StringComparison.OrdinalIgnoreCase))
            ||
            // module:{id}:item:read / location:{id}:item:read gibi desenler
            ctx.User.Claims.Any(c =>
                c.Type.Equals("access", StringComparison.OrdinalIgnoreCase) &&
                c.Value.EndsWith(":item:read", StringComparison.OrdinalIgnoreCase))
        )
);
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseResponseCompression();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapHub<RealtimeHub>("/hubs/realtime"); // <-- EKLENDİ


app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true
});
  
app.UseMiddleware<RequestTimingMiddleware>();

app.Use(async (ctx, next) =>
{
    var userId = ctx.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anon";
    var userName = ctx.User?.Identity?.Name ?? "anonymous";
    var remoteIp = ctx.Connection.RemoteIpAddress?.ToString();

    using (LogContext.PushProperty("UserId", userId))
    using (LogContext.PushProperty("UserName", userName))
    using (LogContext.PushProperty("RemoteIp", remoteIp))
    using (LogContext.PushProperty("TraceId", ctx.TraceIdentifier))
    {
        await next();
    }
});

app.UseSerilogRequestLogging(opts =>
{ 
    opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} -> {StatusCode} in {Elapsed:0.0000} ms";
    opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
    {
        diagCtx.Set("QueryString", httpCtx.Request?.QueryString.Value ?? "");
        diagCtx.Set("UserAgent", httpCtx.Request?.Headers.UserAgent.ToString() ?? "");
    };
});

await using (var scope = app.Services.CreateAsyncScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ISeedService>();
    await seeder.EnsureSeedAsync();
}

app.Run();
