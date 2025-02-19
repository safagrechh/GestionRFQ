using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IService<Worker> _workerService;

        public WorkerController(IService<Worker> workerService)
        {
            _workerService = workerService;
        }

        // GET: api/Worker
        [HttpGet]
        public ActionResult<IEnumerable<WorkerDto>> GetAll()
        {
            var workers = _workerService.GetAll()
                .Select(worker => new WorkerDto
                {
                    Id = worker.Id,
                    Nom = worker.Nom,
                    Role = worker.Role
                })
                .ToList();

            return Ok(workers);
        }

        // GET: api/Worker/{id}
        [HttpGet("{id}")]
        public ActionResult<WorkerDto> Get(int id)
        {
            var worker = _workerService.Get(id);
            if (worker == null)
            {
                return NotFound();
            }

            var workerDto = new WorkerDto
            {
                Id = worker.Id,
                Nom = worker.Nom,
                Role = worker.Role
            };

            return Ok(workerDto);
        }

        // POST: api/Worker/create-material
        [HttpPost("create-materialLeader")]
        public ActionResult<WorkerDto> CreateMaterialWorker([FromBody] CreateWorkerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var worker = new Worker
            {
                Nom = dto.Nom,
                Role = RoleW.Material
            };

            _workerService.Add(worker);

            var workerDto = new WorkerDto
            {
                Id = worker.Id,
                Nom = worker.Nom,
                Role = worker.Role
            };

            return CreatedAtAction(nameof(Get), new { id = worker.Id }, workerDto);
        }

        // POST: api/Worker/create-test
        [HttpPost("create-testLeader")]
        public ActionResult<WorkerDto> CreateTestWorker([FromBody] CreateWorkerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var worker = new Worker
            {
                Nom = dto.Nom,
                Role = RoleW.Test
            };

            _workerService.Add(worker);

            var workerDto = new WorkerDto
            {
                Id = worker.Id,
                Nom = worker.Nom,
                Role = worker.Role
            };

            return CreatedAtAction(nameof(Get), new { id = worker.Id }, workerDto);
        }

        // PUT: api/Worker/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateWorkerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var worker = _workerService.Get(id);
            if (worker == null)
            {
                return NotFound();
            }

            // Only update fields if they are provided
            if (dto.Nom != null)
            {
                worker.Nom = dto.Nom;
            }

            if (dto.Role.HasValue)
            {
                worker.Role = dto.Role.Value;
            }

            _workerService.Update(worker);

            return NoContent();
        }


        // DELETE: api/Worker/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var worker = _workerService.Get(id);
            if (worker == null)
            {
                return NotFound();
            }

            _workerService.Delete(worker);

            return NoContent();
        }
    }

    public class WorkerDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public RoleW Role { get; set; }
    }

    public class CreateWorkerDto
    {
        public string Nom { get; set; }
    }

    public class UpdateWorkerDto
    {
        public string? Nom { get; set; }
        public RoleW? Role { get; set; }
    }
}
