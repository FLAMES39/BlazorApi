using BlazorApi.Data;
using BlazorApi.Models;
using BlazorApi.Interfaces;
using BCrypt;
using BlazorApi.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace BlazorApi.Services
{

    public class AuthService : IAuthService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public AuthService(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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

        public async Task<User> AuthenticateUser(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                return null;

            return user;
        }
        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> updateUserDetails(UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == updateUserDto.Id);
            if(user == null)
            {
                return null;
            }
            user.Names = updateUserDto.Names;
            user.Email = updateUserDto.Email;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.Description = updateUserDto.Description;

            await _context.SaveChangesAsync();
            return user;
        }
    }

}
