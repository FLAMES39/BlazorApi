using BlazorApi.Data;
using BlazorApi.Models;
using BlazorApi.Interfaces;
using BCrypt;
using BlazorApi.DTO;
using Microsoft.AspNetCore.Http.HttpResults;


namespace BlazorApi.Services
{

    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        public AuthService(DataContext context) {
            _context = context;
        }

        public async Task<User>CreateUser(UserDto userDTO)
        {
            var isExisting = _context.Users.Any(e => e.Email == userDTO.Email);
            if (isExisting) {
                Console.WriteLine("user Exists");
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
            var user = new User
            {
                Names = userDTO.Names,
                Email = userDTO.Email,
                Password = hashedPassword,
                Role = "user",
                PhoneNumber = userDTO.PhoneNumber
                

            };
            
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }   
    }

}
