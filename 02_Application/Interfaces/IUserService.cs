using _02_Application.Dtos;

namespace _02_Application.Interfaces;

public interface IUserService
{
    Task<UserListDto?> GetByIdAsync(Guid id);
    Task<List<UserListDto>> GetAllAsync();
    Task AddAsync(UserDto dto);
    Task UpdateAsync(UserDto dto);
    Task ChangePasswordAsync(UserChangePasswordDto dto);
    Task DeleteAsync(Guid id);
}