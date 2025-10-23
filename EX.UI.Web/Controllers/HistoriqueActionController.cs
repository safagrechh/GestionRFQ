using System;
using System.Collections.Generic;
using System.Linq;
using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoriqueActionController : ControllerBase
    {
        private readonly IService<HistoriqueAction> _historiqueActionService;
        private readonly IService<User> _userService;

        public HistoriqueActionController(
            IService<HistoriqueAction> historiqueActionService,
            IService<User> userService)
        {
            _historiqueActionService = historiqueActionService;
            _userService = userService;
        }

        // GET: api/HistoriqueAction
        [HttpGet]
        public ActionResult<IEnumerable<HistoriqueActionDto>> GetAll()
        {
            var actions = BuildDtos(_historiqueActionService
                .GetAll()
                .OrderByDescending(a => a.DateAction));

            return Ok(actions);
        }

        // GET: api/HistoriqueAction/filter
        [HttpGet("filter")]
        public ActionResult<IEnumerable<HistoriqueActionDto>> Filter([FromQuery] HistoriqueActionFilter filter)
        {
            var filteredActions = BuildDtos(ApplyFilter(filter));
            return Ok(filteredActions);
        }

        // GET: api/HistoriqueAction/user/{userId}
        [HttpGet("user/{userId}")]
        public ActionResult<IEnumerable<HistoriqueActionDto>> GetByUser(int userId)
        {
            var filter = new HistoriqueActionFilter { UserId = userId };
            var actions = BuildDtos(ApplyFilter(filter));
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

            var actionDto = MapToDto(action, new Dictionary<int, string?>());

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

        private IEnumerable<HistoriqueAction> ApplyFilter(HistoriqueActionFilter filter)
        {
            var actions = _historiqueActionService.GetAll().AsEnumerable();

            // Client-based filtering removed per request

            if (filter.UserId.HasValue)
            {
                actions = actions.Where(a => a.UserId == filter.UserId.Value);
            }

            if (filter.StartDate.HasValue)
            {
                var start = filter.StartDate.Value.Date;
                actions = actions.Where(a => a.DateAction >= start);
            }

            if (filter.EndDate.HasValue)
            {
                var inclusiveEnd = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                actions = actions.Where(a => a.DateAction <= inclusiveEnd);
            }

            if (filter.CQ.HasValue)
            {
                var cqValue = filter.CQ.Value.ToString();
                actions = actions.Where(a => string.Equals(a.ReferenceCible, cqValue, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.CibleAction))
            {
                actions = actions.Where(a => a.CibleAction != null &&
                    a.CibleAction.Equals(filter.CibleAction, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                actions = actions.Where(a => a.Type != null &&
                    a.Type.Equals(filter.Type, StringComparison.OrdinalIgnoreCase));
            }

            return actions.OrderByDescending(a => a.DateAction);
        }

        private IEnumerable<HistoriqueActionDto> BuildDtos(IEnumerable<HistoriqueAction> actions)
        {
            var cache = new Dictionary<int, string?>();
            foreach (var action in actions)
            {
                yield return MapToDto(action, cache);
            }
        }

        private HistoriqueActionDto MapToDto(HistoriqueAction action, IDictionary<int, string?> cache)
        {
            if (!cache.TryGetValue(action.UserId, out var userName))
            {
                userName = _userService.Get(action.UserId)?.NomUser;
                cache[action.UserId] = userName;
            }

            return new HistoriqueActionDto
            {
                Id = action.Id,
                Type = action.Type,
                CibleAction = action.CibleAction,
                ReferenceCible = action.ReferenceCible,
                DetailsAction = action.DetailsAction,
                DateAction = action.DateAction,
                UserId = action.UserId,
                UserName = userName
            };
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
        public string? UserName { get; set; }
    }

    public class HistoriqueActionFilter
    {
        public int? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CQ { get; set; }
        public string? CibleAction { get; set; }
        public string? Type { get; set; }
    }
}
