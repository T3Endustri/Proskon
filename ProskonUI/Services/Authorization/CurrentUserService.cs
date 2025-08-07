using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using _02_Application.Dtos;
using _02_Application.Interfaces;

namespace ProskonUI.Services.Authorization;

public class CurrentUserService(AuthenticationStateProvider authStateProvider, IUserService userService) : ICurrentUserService
{
    private readonly AuthenticationStateProvider _authStateProvider = authStateProvider;
    private readonly IUserService _userService = userService;

    private UserListDto? _cachedUser;

    public UserListDto? User => _cachedUser;

    public bool IsAuthenticated => _cachedUser is not null;

    public async Task EnsureUserLoadedAsync()
    {
        if (_cachedUser is not null)
            return;

        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;

        if (!principal.Identity?.IsAuthenticated ?? true)
            return;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return;

        _cachedUser = await _userService.GetByIdAsync(userId);
    }

    public bool HasClaim(string type, string value)
    {
        if (_cachedUser is null) return false;

        return _cachedUser.Claims.Any(c =>
            string.Equals(c.Type, type, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(c.Value, value, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasRole(string roleName)
    {
        if (_cachedUser is null) return false;

        return _cachedUser.Roles.Any(r =>
            string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase));
    }
}
