using _02_Application.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ProskonUI.Services.Authorization;

public interface IT3AuthService
{
    Task SignInAsync(UserListDto user, bool rememberMe);
    Task SignOutAsync();
}

public class T3AuthService(IHttpContextAccessor http) : IT3AuthService
{
    public async Task SignInAsync(UserListDto user, bool rememberMe)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.UserId) ? user.Email : user.UserId)
        };

        if (user.Roles is not null)
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));

        if (user.Claims is not null)
            claims.AddRange(user.Claims.Select(c => new Claim(c.Type, c.Value)));

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        var props = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        var ctx = http.HttpContext ?? throw new InvalidOperationException("HttpContext yok.");
        await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
    }

    public Task SignOutAsync()
    {
        var ctx = http.HttpContext ?? throw new InvalidOperationException("HttpContext yok.");
        return ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}