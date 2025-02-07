using BlazorApi.Data;
using BlazorApi.Models;
using System.Linq;
using BCrypt.Net;
using System.Threading.Tasks;

namespace BlazorApi.Services
{

    using BCrypt;
    
    
    public class AuthService
    {
        private readonly DataContext _context;
        public AuthService(DataContext context) {
            _context = context;
        }

        public async Task<User>(string Names, string Password , string Description , string Email, string Role, int PhoneNumber)
        {
            var isExisting = _context.Users.Any(e => e.Email == Email);
            if (isExisting) {
                Console.WriteLine("User is Existing");
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
            var user = new Person
            {
                Names = Names,
                Password = hashedPassword,
                Description = Description,
                Role = Role,
                PhoneNumber = PhoneNumber

            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }   
    }

}
