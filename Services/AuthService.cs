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
using static System.Net.Mime.MediaTypeNames;


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

        public async Task<User> CreateUser(UserDto userDTO)
        {
            try
            {
                var isExisting = await _context.Users.AnyAsync(e => e.Email == userDTO.Email);
                if (isExisting)
                {
                    throw new Exception("User with this email already exists.");
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
                Console.WriteLine(user);
                 await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DbUpdateException: {ex.Message}");
                throw;
            }
        }


        //public async Task<User> AuthenticateUser(LoginDto loginDto)
        // {
        //   var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        // if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
        //      return null;

        //  return user;
        // }

        public async Task<User> AuthenticateUser(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                return null; // User not found
            }
           
            if (BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return user;
            }

            var application = await _context.Applications.SingleOrDefaultAsync(a => a.UserId == user.UserId);

            if (application != null && application.TemporaryPassword == loginDto.Password)
            {
                // 3. Check if temporary password is still valid
                if (application.TempPasswordExpiry.HasValue && application.TempPasswordExpiry.Value > DateTime.UtcNow)
                {
                    return user; // Allow login using temporary password
                }
                else
                {
                    // 4. Temporary password expired - remove it
                    application.TemporaryPassword = null;
                    application.TempPasswordExpiry = null;
                    await _context.SaveChangesAsync();
                }
            }

            return null; 
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



        public async Task<bool> DeleteUser(int UserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == UserId);
            if (user == null) return false;


            user.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<User> updateUserDetails(UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == updateUserDto.UserId);
            if (user == null)
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

        public async Task<User> GetUserById(int UserId)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()  // Ensures read-only access for performance
                    .FirstOrDefaultAsync(u => u.UserId == UserId && !u.IsDeleted);

                if (user == null) Console.WriteLine($"User with ID {UserId} not found.");

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user: {ex.Message}");
                throw;
            }
        }

        public async Task<List<UserDetailDto>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Where(u => !u.IsDeleted)
                    .Select(u => new UserDetailDto
                    {
                        UserId = u.UserId,
                        Names = u.Names,
                        Email = u.Email,
                        Role = u.Role,
                        PhoneNumber = u.PhoneNumber,
                        Description = u.Description
                    })
                    .ToListAsync();

                return users;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all users: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ValidateTemporaryPassword(string email, string tempPassword)
            {
                var user = await _context.Applications.FirstOrDefaultAsync(a => a.Email == email);

                if (user == null)
                {
                    return false; // User not found
                }

                if (user.TemporaryPassword != tempPassword || user.TempPasswordExpiry < DateTime.UtcNow)
                {
                    return false; // Password incorrect or expired
                }

                return true;
            }



    }

}