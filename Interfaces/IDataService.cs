using BlazorApi.DTO;
using BlazorApi.Models;

namespace BlazorApi.Interfaces
{
    public interface IDataService
    {
        Task<List<User>> GetAllUsers();

        Task<User> GetUserById(int UserId);
        Task<User> CreateUser(UserDto userDto);
        Task<User> UpdateUserDetails(UpdateUserDto updateUserDto);
        Task<bool> DeleteUser(int UserId);
    }
}
