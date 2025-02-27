using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class UserController : ControllerBase
    {
        private readonly IService<User> _userService;
        private readonly TokenService _tokenService;

        public UserController(IService<User> userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]//  Exemple : tu peux autoriser certaines routes publiquement si besoin
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                var emailClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userByEmail = _userService.GetAll().FirstOrDefault(u => u.Email == emailClaim);
                if (userByEmail == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new
                {
                    userByEmail.Id,
                    userByEmail.Email,
                    userByEmail.Role
                });
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(new { message = "Invalid user ID format in token" });
            }

            var user = _userService.Get(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new
            {
                user.Id,
                user.Email,
                user.NomUser,
                user.Role,

            });
        }

        [HttpGet("admin")]
        public IActionResult AdminEndpoint()
        {
            return Ok(new { message = "Welcome, Admin!" });
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserSummaryDto>> GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = users.Select(user => new UserSummaryDto
            {
                Id = user.Id,
                NomUser = user.NomUser,
                Email = user.Email
            }).ToList();
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<UserSummaryDto> Get(int id)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserSummaryDto
            {
                Id = user.Id,
                NomUser = user.NomUser,
                Email = user.Email
            };

            return Ok(userDto);
        }


        [HttpPost]
        public ActionResult<User> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                NomUser = dto.NomUser,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role,
                Commentaires = new List<Commentaire>(),
                HistoriqueActions = new List<HistoriqueAction>(),
                Rapports = new List<Rapport>(),
                RFQsEnTantQueIngenieur = new List<RFQ>(),
                RFQsEnTantQueValidateur = new List<RFQ>(),
                VersionRFQsEnTantQueIngenieur = new List<VersionRFQ>(),
                VersionRFQsEnTantQueValidateur = new List<VersionRFQ>()
            };

            _userService.Add(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("User ID mismatch");
            }

            var existingUser = _userService.Get(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.NomUser = dto.NomUser;
            existingUser.Email = dto.Email;
            existingUser.Password = dto.Password;
            existingUser.Role = dto.Role;

            _userService.Update(existingUser);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _userService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            _userService.Delete(user);

            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("by-role/{role}")]
        public ActionResult<IEnumerable<UserSummaryDto>> GetUsersByRole(string role)
        {
            // Fetch all users with the specified role
            var users = _userService.GetAll().Where(u => u.Role.ToString() == role).ToList();

            if (users == null || !users.Any())
            {
                return NotFound(new { message = "No users found with the specified role." });
            }

            // Map the list of users to a UserSummaryDto
            var userDtos = users.Select(user => new UserSummaryDto
            {
                Id = user.Id,
                NomUser = user.NomUser,
                Email = user.Email
            }).ToList();

            return Ok(userDtos);
        }

    }


    public class CreateUserDto
    {
        public string NomUser { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleU Role { get; set; }
    }

    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string NomUser { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RoleU Role { get; set; }
    }

    public class UserSummaryDto
    {
        public int Id { get; set; }
        public string NomUser { get; set; }
        public string Email { get; set; }
    }
}
