using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RFQController : ControllerBase
    {
        private readonly IService<RFQ> _rfqService;

        public RFQController(IService<RFQ> rfqService)
        {
            _rfqService = rfqService;
        }

        // GET: api/RFQ
        [HttpGet]
        public ActionResult<IEnumerable<RFQ>> GetAll()
        {
            var rfqs = _rfqService.GetAll();
            return Ok(rfqs);
        }

        // GET: api/RFQ/5
        [HttpGet("{id}")]
        public ActionResult<RFQ> Get(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }
            return Ok(rfq);
        }

        // POST: api/RFQ
        [HttpPost]
        public ActionResult<RFQ> Create([FromBody] RFQ rfq)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _rfqService.Add(rfq);
            return CreatedAtAction(nameof(Get), new { id = rfq.CodeRFQ }, rfq);
        }

        // PUT: api/RFQ/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] RFQ rfq)
        {
            if (id != rfq.CodeRFQ)
            {
                return BadRequest();
            }

            _rfqService.Update(rfq);
            return NoContent();
        }

        // DELETE: api/RFQ/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            _rfqService.Delete(rfq);
            return NoContent();
        }
    }
}
