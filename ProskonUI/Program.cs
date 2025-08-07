using ProskonUI.Components;
using ProskonUI.Services.Authorization;

var builder = WebApplication.CreateBuilder(args);
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mzk4MzIwN0AzMzMwMmUzMDJlMzAzYjMzMzAzYmZLUGwvQ2J5eTl1Ums0dFJsZU41d1lSMHJWZU5kbHc2YTEvazQveTVBVVU9\r\n");


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
