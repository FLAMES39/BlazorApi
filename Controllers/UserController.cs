using Microsoft.AspNetCore.Mvc;
using BlazorApi.Services;
using BlazorApi.DTO;
using BlazorApi.Models;
using System.Security.Claims;

namespace BlazorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private AuthService _AuthService;
        public UserController(AuthService AuthService)
        {
            _AuthService = AuthService;
        }
        [HttpPost("/register")]
        public async Task<ActionResult<User>> RegisterUser(UserDto userDto)
        {
            try
            {
                var createdUser = await _AuthService.CreateUser(userDto);
                if (createdUser != null)
                {
                    return Ok(new { Message = "Registration successful", UserId = createdUser.UserId });
                }
                return BadRequest(new { Message = "Registration failed" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error" });
            }
        }

        [HttpPost("/login")]
        public async Task<ActionResult<User>> loginUser(LoginDto loginDto)

        {
            try
            {
                var user = await _AuthService.AuthenticateUser(loginDto);
                if (user == null) return Unauthorized("Invalid credentials");

                var token = _AuthService.GenerateJwtToken(user);
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    UserId = user.UserId,
                    Role = user.Role 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

        }


        [HttpDelete("Delete/{UserId}")]
        public async Task<ActionResult<bool>> DeleteUser(int UserId)
        {
            try
            {
                var user = await _AuthService.DeleteUser(UserId);
                if (!user)
                {
                    return Unauthorized("User Doesn't Exist");
                }
                return Ok("User Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Internal Server Error.", error = ex.Message });
            }
        }


        [HttpPut("/update")]
        public async Task<ActionResult<User>> updateUser(UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _AuthService.updateUserDetails(updateUserDto);
                if (user != null)
                {
                    return Ok(new { Message = "User updated successfully", UpdatedUser = user });
                }
                return NotFound(new { Message = "User not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
        [HttpGet("{UserId}")]
        public async Task<ActionResult<UserDetailDto>> GetUserById(int UserId)
        {
            try
            {
                var user = await _AuthService.GetUserById(UserId);
                if (user == null) return NotFound(new { Message = "User not found." });

                return Ok(user);  // Return UserDetailDto
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }


        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<UserDetailDto>>> GetAllUsers()
        {
            try
            {
                var users = await _AuthService.GetAllUsers();
                if (users == null || !users.Any())
                {
                    return NotFound(new { Message = "No users found." });
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [HttpGet("GetCurrentUserId")]
        public IActionResult GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Using claims for authentication
            return Ok(userId);
        }


    }
}