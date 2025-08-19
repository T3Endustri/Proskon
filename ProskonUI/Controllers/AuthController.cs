using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using _02_Application.Interfaces;
using _02_Application.Dtos;

namespace ProskonUI.Controllers;

[ApiController]
[Route("api/[controller]")]
[IgnoreAntiforgeryToken] // Program.cs'de UseAntiforgery var; Blazor içinden kolay çağrı için
public class AuthController(IAuthService auth) : ControllerBase
{
    private readonly IAuthService _auth = auth;

    [HttpPost("signin")]
    public async Task<IActionResult> UserSignIn([FromBody] LoginDto dto, [FromQuery] bool rememberMe = true)
    {
        var result = await _auth.LoginAsync(dto);
         

        if (!result.Success || result.User is null)
            return BadRequest(result.Message ?? "Geçersiz kullanıcı veya şifre.");

        var u = result.User;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(u.UserId) ? u.Email : u.UserId)
        };

        if (u.Roles is not null)
            claims.AddRange(u.Roles.Select(r => new Claim(ClaimTypes.Role, r.Name)));
        if (u.Claims is not null)
            claims.AddRange(u.Claims.Select(c => new Claim(c.Type, c.Value)));

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

        var props = new AuthenticationProperties
        {
            IsPersistent = rememberMe,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

        return Ok(new { startPage = u.StartPage ?? "/" });
    }

    [HttpGet("signout")]
    public async Task<IActionResult> UserSignOutGet()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return LocalRedirect("/login");
    }
}
