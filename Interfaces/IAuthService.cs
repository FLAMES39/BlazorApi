using BlazorApi.DTO;
using BlazorApi.Models;

namespace BlazorApi.Interfaces
{
    public interface IAuthService
    {
        Task<User> CreateUser(UserDto userDto);
        Task<User> AuthenticateUser(LoginDto loginDto); 
        public string GenerateJwtToken(User user);
        Task<bool> DeleteUser(int UserId);
    }
}
