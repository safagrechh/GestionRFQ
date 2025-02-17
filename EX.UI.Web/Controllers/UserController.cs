using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IService<User> _userService;

        public UserController(IService<User> userService)
        {
            _userService = userService;
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
