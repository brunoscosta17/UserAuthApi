using UserAuthApi.DTOs;
using UserAuthApi.Models;

namespace UserAuthApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterUserDto dto);
        Task<User?> AuthenticateAsync(LoginUserDto dto);
        Task<User?> GetByEmailAsync(string email);
        string GenerateRefreshToken();
        Task<User> UpdateAsync(User user);
    }
}
