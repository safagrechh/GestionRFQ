using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IService<User> _userService;
        private readonly TokenService _tokenService;

        public AuthController(IService<User> userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _userService.GetAll().FirstOrDefault(u => u.Email == loginDto.Email && u.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = _tokenService.GenerateToken(user);

            return Ok(new { token });
        }


    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
