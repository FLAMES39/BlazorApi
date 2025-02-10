using Microsoft.AspNetCore.Mvc;
using BlazorApi.Services;
using BlazorApi.DTO;
using BlazorApi.Models;

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
        public async Task<ActionResult<User>> registerUser(UserDto userDto)

        {
            try
            {
                var results = await _AuthService.CreateUser(userDto);
                return Ok(results);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        [HttpPut("/update")]
        public async Task<ActionResult<User>> updateUser(UpdateUserDto updateUserDto)

        {
            try
            {
               var result = await _AuthService.updateUserDetails(updateUserDto);
               return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}