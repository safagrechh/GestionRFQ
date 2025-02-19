using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriqueActionController : ControllerBase
    {
        private readonly IService<HistoriqueAction> _historiqueActionService;

        public HistoriqueActionController(IService<HistoriqueAction> historiqueActionService)
        {
            _historiqueActionService = historiqueActionService;
        }

        // GET: api/HistoriqueAction
        [HttpGet]
        public ActionResult<IEnumerable<HistoriqueActionDto>> GetAll()
        {
            var actions = _historiqueActionService.GetAll()
                .Select(a => new HistoriqueActionDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    CibleAction = a.CibleAction,
                    ReferenceCible = a.ReferenceCible,
                    DetailsAction = a.DetailsAction,
                    DateAction = a.DateAction,
                    UserId = a.UserId
                });
            return Ok(actions);
        }

        // GET: api/HistoriqueAction/user/{userId}
        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<HistoriqueActionDto>> GetByUser(int userId)
        {
            var actions = _historiqueActionService.GetAll()
                .Where(a => a.UserId == userId)
                .Select(a => new HistoriqueActionDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    CibleAction = a.CibleAction,
                    ReferenceCible = a.ReferenceCible,
                    DetailsAction = a.DetailsAction,
                    DateAction = a.DateAction,
                    UserId = a.UserId
                });
            return Ok(actions);
        }

        // POST: api/HistoriqueAction
        [HttpPost]
        public ActionResult<HistoriqueActionDto> AddAction([FromBody] CreateHistoriqueActionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var action = new HistoriqueAction
            {
                Type = dto.Type,
                CibleAction = dto.CibleAction,
                ReferenceCible = dto.ReferenceCible,
                DetailsAction = dto.DetailsAction,
                DateAction = dto.DateAction,
                UserId = dto.UserId
            };

            _historiqueActionService.Add(action);

            var actionDto = new HistoriqueActionDto
            {
                Id = action.Id,
                Type = action.Type,
                CibleAction = action.CibleAction,
                ReferenceCible = action.ReferenceCible,
                DetailsAction = action.DetailsAction,
                DateAction = action.DateAction,
                UserId = action.UserId
            };

            return CreatedAtAction(nameof(GetAll), new { id = action.Id }, actionDto);
        }

        // DELETE: api/HistoriqueAction/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var action = _historiqueActionService.Get(id);
            if (action == null)
            {
                return NotFound();
            }

            _historiqueActionService.Delete(action);

            return NoContent();
        }
    }

    public class CreateHistoriqueActionDto
    {
        public string Type { get; set; }
        public string CibleAction { get; set; }
        public string ReferenceCible { get; set; }
        public string DetailsAction { get; set; }
        public DateTime DateAction { get; set; }
        public int UserId { get; set; }
    }

    public class HistoriqueActionDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string CibleAction { get; set; }
        public string ReferenceCible { get; set; }
        public string DetailsAction { get; set; }
        public DateTime DateAction { get; set; }
        public int UserId { get; set; }
    }
}
