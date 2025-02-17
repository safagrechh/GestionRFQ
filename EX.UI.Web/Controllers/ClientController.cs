using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IService<Client> _clientService;

        public ClientController(IService<Client> clientService)
        {
            _clientService = clientService;
        }

        // GET: api/Client
        [HttpGet]
        public ActionResult<IEnumerable<ClientSummaryDto>> GetAll()
        {
            var clients = _clientService.GetAll();
            var clientDtos = clients.Select(client => new ClientSummaryDto
            {
                Id = client.Id,
                Nom = client.Nom,
                Email = client.Email,
                Sales = client.Sales
            }).ToList();
            return Ok(clientDtos);
        }

        // GET: api/Client/{id}
        [HttpGet("{id}")]
        public ActionResult<ClientDetailsDto> Get(int id)
        {
            var client = _clientService.Get(id);
            if (client == null)
            {
                return NotFound();
            }

            var clientDto = new ClientDetailsDto
            {
                Id = client.Id,
                Nom = client.Nom,
                Email = client.Email,
                Sales = client.Sales,
                RFQs = client.RFQs?.Select(rfq => new RFQSummaryDto
                {
                    QuoteName = rfq.QuoteName,
                    CodeRFQ = rfq.CodeRFQ
                }).ToList()
            };

            return Ok(clientDto);
        }

        // POST: api/Client
        [HttpPost]
        public ActionResult<Client> Create([FromBody] CreateClientDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = new Client
            {
                Nom = dto.Nom,
                Email = dto.Email,
                Sales = dto.Sales
            };

            _clientService.Add(client);
            return CreatedAtAction(nameof(Get), new { id = client.Id }, client);
        }

        // PUT: api/Client/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateClientDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("Client ID mismatch");
            }

            var existingClient = _clientService.Get(id);
            if (existingClient == null)
            {
                return NotFound();
            }

            existingClient.Nom = dto.Nom;
            existingClient.Email = dto.Email;
            existingClient.Sales = dto.Sales;

            _clientService.Update(existingClient);
            return NoContent();
        }

        // DELETE: api/Client/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var client = _clientService.Get(id);
            if (client == null)
            {
                return NotFound();
            }

            _clientService.Delete(client);
            return NoContent();
        }
    }

    // DTOs for the Client API
    public class CreateClientDto
    {
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Sales { get; set; }
    }

    public class UpdateClientDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Sales { get; set; }
    }

    public class ClientSummaryDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
        public string Sales { get; set; }
    }

    public class ClientDetailsDto : ClientSummaryDto
    {
        public List<RFQSummaryDto> RFQs { get; set; }
    }

    public class RFQSummaryDto
    {
         public int CodeRFQ { get; set; }
        public string QuoteName { get; set; }
        
    }
}
