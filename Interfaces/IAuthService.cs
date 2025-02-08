using BlazorApi.DTO;
using BlazorApi.Models;

namespace BlazorApi.Interfaces
{
    public interface IAuthService
    {
        Task<User> CreateUser(UserDto userDto);
    }
}
