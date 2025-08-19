// ProskonUI/Services/Authorization/CurrentUserService.cs
using _02_Application.Dtos;
using _02_Application.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ProskonUI.Services.Authorization;

public class CurrentUserService : ICurrentUser
{
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IUserService _userService;
    private UserListDto? _cachedUser; 
    public UserListDto? User => _cachedUser;


    public CurrentUserService(AuthenticationStateProvider authStateProvider, IUserService userService)
    {
        _authStateProvider = authStateProvider;
        _userService = userService;

        _authStateProvider.AuthenticationStateChanged += async task =>
        {
            try
            {
                var state = await task;
                var principal = state.User;

                if (principal.Identity?.IsAuthenticated == true)
                {
                    var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(id, out var userId))
                        _cachedUser = await _userService.GetByIdAsync(userId);
                }
                else
                {
                    _cachedUser = null;
                }
            }
            catch
            {
                _cachedUser = null;
            }
        };
    }

    public async Task EnsureLoadedAsync(CancellationToken ct = default)
    {
        if (_cachedUser is not null) return;

        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var principal = authState.User;
        if (principal.Identity?.IsAuthenticated != true) return;

        var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(id, out var userId))
            _cachedUser = await _userService.GetByIdAsync(userId);
    }

    public Guid? UserId => _cachedUser?.Id;
    public bool IsAuthenticated => _cachedUser is not null;

    public IReadOnlyCollection<string> Roles
        => _cachedUser?.Roles.Select(r => r.Name).ToArray() ?? [];

    public IReadOnlyCollection<(string Type, string Value)> Claims
        => _cachedUser?.Claims.Select(c => (c.Type, c.Value)).ToArray()
           ?? [];

    public bool HasRole(string roleName)
        => Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);

    public bool HasClaim(string type, string value)
        => Claims.Any(c =>
            string.Equals(c.Type, type, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(c.Value, value, StringComparison.OrdinalIgnoreCase));
}
