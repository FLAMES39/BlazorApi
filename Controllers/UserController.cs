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
    }
}