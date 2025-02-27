using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketSegmentController : ControllerBase
    {
        private readonly IService<MarketSegment> _marketSegmentService;

        public MarketSegmentController(IService<MarketSegment> marketSegmentService)
        {
            _marketSegmentService = marketSegmentService;
        }

        // GET: api/MarketSegment
        [HttpGet]
        public ActionResult<IEnumerable<MarketSegmentDto>> GetAll()
        {
            var marketSegments = _marketSegmentService.GetAll()
                .Select(m => new MarketSegmentDto
                {
                    Id = m.Id,
                    Nom = m.Nom,
                  
                })
                .ToList(); ;
            return Ok(marketSegments);
        }

        // GET: api/MarketSegment/{id}
        [HttpGet("{id}")]
        public ActionResult<MarketSegment> Get(int id)
        {
            var marketSegment = _marketSegmentService.Get(id);
            if (marketSegment == null)
            {
                return NotFound();
            }
            return Ok(marketSegment);
        }

        // POST: api/MarketSegment
        [HttpPost]
        public ActionResult<MarketSegment> Create([FromBody] MarketSegment marketSegment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _marketSegmentService.Add(marketSegment);
            return CreatedAtAction(nameof(Get), new { id = marketSegment.Id }, marketSegment);
        }

        // PUT: api/MarketSegment/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MarketSegment marketSegment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingMarketSegment = _marketSegmentService.Get(id);
            if (existingMarketSegment == null)
            {
                return NotFound();
            }

            existingMarketSegment.Nom = marketSegment.Nom;

            _marketSegmentService.Update(existingMarketSegment);

            return NoContent();
        }

        // DELETE: api/MarketSegment/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var marketSegment = _marketSegmentService.Get(id);
            if (marketSegment == null)
            {
                return NotFound();
            }

            _marketSegmentService.Delete(marketSegment);
            return NoContent();
        }
    }
    public class MarketSegmentDto
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        
    }
}
