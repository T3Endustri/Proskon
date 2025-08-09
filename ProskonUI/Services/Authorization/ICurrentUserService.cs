using _02_Application.Dtos;

namespace ProskonUI.Services.Authorization;

public interface ICurrentUserService
{
    /// <summary>
    /// Giriş yapan kullanıcının tüm bilgilerini içerir.
    /// Roller, claimler, ad-soyad, email vb.
    /// </summary>
    UserListDto? User { get; }

    /// <summary>
    /// Kullanıcı giriş yapmış mı?
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Kullanıcının belirli bir claim'e sahip olup olmadığını kontrol eder.
    /// </summary>
    bool HasClaim(string type, string value);

    /// <summary>
    /// Kullanıcının belirli bir role sahip olup olmadığını kontrol eder.
    /// </summary>
    bool HasRole(string roleName);

    Task EnsureUserLoadedAsync();
}
