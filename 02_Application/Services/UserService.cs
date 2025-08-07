using _01_Data.Entities;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Dtos;
using _02_Application.Interfaces;
using AutoMapper;

namespace _02_Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserListDto?> GetByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Repository<T3IdentityUser>().GetByIdAsync(id, u => u.ListRoles, u => u.ListClaims);
        return user is null ? null : _mapper.Map<UserListDto>(user);
    }

    public async Task<List<UserListDto>> GetAllAsync()
    {
        var users = await _unitOfWork.Repository<T3IdentityUser>().ListAsync(UserSpec.All());
        return _mapper.Map<List<UserListDto>>(users);
    }

    public async Task AddAsync(UserDto dto)
    {
        var user = _mapper.Map<T3IdentityUser>(dto);
        await _unitOfWork.Repository<T3IdentityUser>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserDto dto)
    {
        var user = await _unitOfWork.Repository<T3IdentityUser>().GetByIdAsync(dto.Id);
        if (user is null)
            throw new Exception("Kullanıcı bulunamadı");

        _mapper.Map(dto, user);
        await _unitOfWork.Repository<T3IdentityUser>().UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(UserChangePasswordDto dto)
    {
        var user = await _unitOfWork.Repository<T3IdentityUser>().GetByIdAsync(dto.Id);
        if (user is null)
            throw new Exception("Kullanıcı bulunamadı");

        user.PasswordHash = HashPassword(dto.NewPassword);
        await _unitOfWork.Repository<T3IdentityUser>().UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _unitOfWork.Repository<T3IdentityUser>().DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    private string HashPassword(string password)
    {
        // Gerçek uygulamada şifreyi hash'lemen gerekir.
        return password;
    }
}