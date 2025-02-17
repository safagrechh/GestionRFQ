using EX.Core.Domain;
using EX.Core.Services;
using Humanizer;
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
        public ActionResult<RFQ> Create([FromBody] CreateRFQDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rfq = new RFQ
            {
                QuoteName = dto.QuoteName,
                NumRefQuoted = dto.NumRefQuoted,
                SOPDate = dto.SOPDate,
                MaxV = dto.MaxV,
                EstV = dto.EstV,
                KODate = dto.KODate,
                CustomerDataDate = dto.CustomerDataDate,
                MDDate = dto.MDDate,
                MRDate = dto.MRDate,
                TDDate = dto.TDDate,
                TRDate = dto.TRDate,
                LDDate = dto.LDDate,
                LRDate = dto.LRDate,
                CDDate = dto.CDDate,
                ApprovalDate = dto.ApprovalDate,
                DateCreation = dto.DateCreation,
                Statut = dto.Statut,

                MaterialLeaderId = dto.MaterialLeaderId,
                TestLeaderId = dto.TestLeaderId,
                MarketSegmentId = dto.MarketSegmentId,
                ClientId = dto.ClientId,
                IngenieurRFQId = dto.IngenieurRFQId,
                ValidateurId = dto.ValidateurId
            };

            _rfqService.Add(rfq);

            return CreatedAtAction(nameof(Get), new { id = rfq.CodeRFQ }, rfq);
        }


        // PUT: api/RFQ/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRFQDto dto)
        {
            if (id != dto.CodeRFQ)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingRFQ = _rfqService.Get(id);
            if (existingRFQ == null)
            {
                return NotFound();
            }

            existingRFQ.QuoteName = dto.QuoteName;
            existingRFQ.NumRefQuoted = dto.NumRefQuoted;
            existingRFQ.SOPDate = dto.SOPDate;
            existingRFQ.MaxV = dto.MaxV;
            existingRFQ.EstV = dto.EstV;
            existingRFQ.KODate = dto.KODate;
            existingRFQ.CustomerDataDate = dto.CustomerDataDate;
            existingRFQ.MDDate = dto.MDDate;
            existingRFQ.MRDate = dto.MRDate;
            existingRFQ.TDDate = dto.TDDate;
            existingRFQ.TRDate = dto.TRDate;
            existingRFQ.LDDate = dto.LDDate;
            existingRFQ.LRDate = dto.LRDate;
            existingRFQ.CDDate = dto.CDDate;
            existingRFQ.ApprovalDate = dto.ApprovalDate;
            existingRFQ.DateCreation = dto.DateCreation;
            existingRFQ.Statut = dto.Statut;
            existingRFQ.MaterialLeaderId = dto.MaterialLeaderId;
            existingRFQ.TestLeaderId = dto.TestLeaderId;
            existingRFQ.MarketSegmentId = dto.MarketSegmentId;
            existingRFQ.ClientId = dto.ClientId;
            existingRFQ.IngenieurRFQId = dto.IngenieurRFQId;
            existingRFQ.ValidateurId = dto.ValidateurId;

            _rfqService.Update(existingRFQ);

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
        public class CreateRFQDto
        {
            public string QuoteName { get; set; }
            public int NumRefQuoted { get; set; }
            public DateTime? SOPDate { get; set; }
            public int MaxV { get; set; }
            public int EstV { get; set; }
            public DateTime? KODate { get; set; }
            public DateTime? CustomerDataDate { get; set; }
            public DateTime? MDDate { get; set; }
            public DateTime? MRDate { get; set; }
            public DateTime? TDDate { get; set; }
            public DateTime? TRDate { get; set; }
            public DateTime? LDDate { get; set; }
            public DateTime? LRDate { get; set; }
            public DateTime? CDDate { get; set; }
            public DateTime? ApprovalDate { get; set; }
            public DateTime DateCreation { get; set; }
            public Statut Statut { get; set; }

            public int? MaterialLeaderId { get; set; }
            public int? TestLeaderId { get; set; }
            public int? MarketSegmentId { get; set; }
            public int? ClientId { get; set; }
            public int? IngenieurRFQId { get; set; }
            public int? ValidateurId { get; set; }

        }

        public class UpdateRFQDto
    {
        public int CodeRFQ { get; set; }
        public string QuoteName { get; set; }
        public int NumRefQuoted { get; set; }
        public DateTime? SOPDate { get; set; }
        public int MaxV { get; set; }
        public int EstV { get; set; }
        public DateTime? KODate { get; set; }
        public DateTime? CustomerDataDate { get; set; }
        public DateTime? MDDate { get; set; }
        public DateTime? MRDate { get; set; }
        public DateTime? TDDate { get; set; }
        public DateTime? TRDate { get; set; }
        public DateTime? LDDate { get; set; }
        public DateTime? LRDate { get; set; }
        public DateTime? CDDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime DateCreation { get; set; }
        public Statut Statut { get; set; }
        public int? MaterialLeaderId { get; set; }
        public int? TestLeaderId { get; set; }
        public int? MarketSegmentId { get; set; }
        public int? ClientId { get; set; }
        public int? IngenieurRFQId { get; set; }
        public int? ValidateurId { get; set; }
    }

}
