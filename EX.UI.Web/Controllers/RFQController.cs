using EX.Core.Domain;
using EX.Core.Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    [Authorize(Roles = "IngenieurRFQ , Validateur")]
    [Route("api/[controller]")]
    [ApiController]
    public class RFQController : ControllerBase
    {
        private readonly IService<RFQ> _rfqService;

        public RFQController(IService<RFQ> rfqService)
        {
            _rfqService = rfqService;
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpGet]
        public ActionResult<IEnumerable<RFQDetailsDto>> GetAll()
        {
            var rfqs = _rfqService.GetAll();
            var rfqDtos = rfqs.Select(r => new RFQDetailsDto
            {
                CodeRFQ = r.CodeRFQ,
                QuoteName = r.QuoteName,
                NumRefQuoted = r.NumRefQuoted,
                SOPDate = r.SOPDate,
                MaxV = r.MaxV,
                EstV = r.EstV,
                KODate = r.KODate,
                CustomerDataDate = r.CustomerDataDate,
                MDDate = r.MDDate,
                MRDate = r.MRDate,
                TDDate = r.TDDate,
                TRDate = r.TRDate,
                LDDate = r.LDDate,
                LRDate = r.LRDate,
                CDDate = r.CDDate,
                ApprovalDate = r.ApprovalDate,
                DateCreation = r.DateCreation,
                Statut = r.Statut,
                MaterialLeader = r.MaterialLeader?.Nom,
                TestLeader = r.TestLeader?.Nom,
                MarketSegment = r.MarketSegment?.Nom,
                IngenieurRFQ = r.IngenieurRFQ?.NomUser,
                Validateur = r.Validateur?.NomUser,
                Client = r.Client?.Nom
            }).ToList();

            return Ok(rfqDtos);
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpGet("{id}")]
        public ActionResult<RFQDetailsDto> Get(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            var rfqDto = new RFQDetailsDto
            {
                CodeRFQ = rfq.CodeRFQ,
                QuoteName = rfq.QuoteName,
                NumRefQuoted = rfq.NumRefQuoted,
                SOPDate = rfq.SOPDate,
                MaxV = rfq.MaxV,
                EstV = rfq.EstV,
                KODate = rfq.KODate,
                CustomerDataDate = rfq.CustomerDataDate,
                MDDate = rfq.MDDate,
                MRDate = rfq.MRDate,
                TDDate = rfq.TDDate,
                TRDate = rfq.TRDate,
                LDDate = rfq.LDDate,
                LRDate = rfq.LRDate,
                CDDate = rfq.CDDate,
                ApprovalDate = rfq.ApprovalDate,
                DateCreation = rfq.DateCreation,
                Statut = rfq.Statut,
                MaterialLeader = rfq.MaterialLeader?.Nom,
                TestLeader = rfq.TestLeader?.Nom,
                MarketSegment = rfq.MarketSegment?.Nom,
                IngenieurRFQ = rfq.IngenieurRFQ?.NomUser,
                Validateur = rfq.Validateur?.NomUser,
                Client = rfq.Client?.Nom
            };

            return Ok(rfqDto);
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
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
                Statut = dto.Statut ?? Statut.Brouillon, // Default to Brouillon
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
                DateCreation = DateTime.UtcNow,
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

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateRFQDto dto)
        {
            if (id != dto.CodeRFQ)
            {
                return BadRequest("RFQ ID mismatch");
            }

            var existingRFQ = _rfqService.Get(id);
            if (existingRFQ == null)
            {
                return NotFound();
            }

            existingRFQ.QuoteName = dto.QuoteName ?? existingRFQ.QuoteName;
            existingRFQ.NumRefQuoted = dto.NumRefQuoted ?? existingRFQ.NumRefQuoted;
            existingRFQ.SOPDate = dto.SOPDate ?? existingRFQ.SOPDate;
            existingRFQ.MaxV = dto.MaxV ?? existingRFQ.MaxV;
            existingRFQ.EstV = dto.EstV ?? existingRFQ.EstV;
            existingRFQ.Statut = dto.Statut ?? existingRFQ.Statut;
            existingRFQ.KODate = dto.KODate ?? existingRFQ.KODate;
            existingRFQ.CustomerDataDate = dto.CustomerDataDate ?? existingRFQ.CustomerDataDate;
            existingRFQ.MDDate = dto.MDDate ?? existingRFQ.MDDate;
            existingRFQ.MRDate = dto.MRDate ?? existingRFQ.MRDate;
            existingRFQ.TDDate = dto.TDDate ?? existingRFQ.TDDate;
            existingRFQ.TRDate = dto.TRDate ?? existingRFQ.TRDate;
            existingRFQ.LDDate = dto.LDDate ?? existingRFQ.LDDate;
            existingRFQ.LRDate = dto.LRDate ?? existingRFQ.LRDate;
            existingRFQ.CDDate = dto.CDDate ?? existingRFQ.CDDate;
            existingRFQ.ApprovalDate = dto.ApprovalDate ?? existingRFQ.ApprovalDate;
            existingRFQ.MaterialLeaderId = dto.MaterialLeaderId ?? existingRFQ.MaterialLeaderId;
            existingRFQ.TestLeaderId = dto.TestLeaderId ?? existingRFQ.TestLeaderId;
            existingRFQ.MarketSegmentId = dto.MarketSegmentId ?? existingRFQ.MarketSegmentId;
            existingRFQ.ValidateurId = dto.ValidateurId ?? existingRFQ.ValidateurId;
            existingRFQ.IngenieurRFQId = dto.IngenieurRFQId ?? existingRFQ.IngenieurRFQId;

            _rfqService.Update(existingRFQ);

            return NoContent();
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
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

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPost("{id}/finaliser")]
        public IActionResult FinaliserBrouillon(int id)
        {
            var rfq = _rfqService.Get(id);

            if (rfq == null || rfq.Statut != Statut.Brouillon)
                return BadRequest("RFQ non trouvée ou non en brouillon");

            rfq.Statut = Statut.Finalise;
            _rfqService.Update(rfq);

            return Ok(rfq);
        }

        [Authorize(Roles = "Validateur")]
        [HttpPost("{id}/valider")]
        public IActionResult Valider(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            if (rfq.Statut != Statut.Finalise)
            {
                return BadRequest("Seules les RFQs finalisées peuvent être validées.");
            }

            rfq.Statut = Statut.Valide;
            rfq.ApprovalDate = DateTime.UtcNow;
            _rfqService.Update(rfq);

            return Ok(rfq);
        }

        [Authorize(Roles = "Validateur")]
        [HttpPost("{id}/rejeter")]
        public IActionResult Rejeter(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            if (rfq.Statut != Statut.Finalise)
            {
                return BadRequest("Seules les RFQs finalisées peuvent être rejetées.");
            }

            rfq.Statut = Statut.Rejete;
            _rfqService.Update(rfq);

            return Ok(rfq);
        }

        [HttpGet("bystatut/{statut}")]
        public ActionResult<IEnumerable<object>> GetRFQsByStatut(string statut)
        {
            if (!Enum.TryParse<Statut>(statut, true, out var statutEnum))
            {
                return BadRequest($"Invalid statut '{statut}'. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Statut)))}");
            }

            var rfqs = _rfqService.GetAll()
                .Where(r => r.Statut == statutEnum)
                .Select(r => new
                {
                    r.CodeRFQ,
                    r.QuoteName,
                    r.NumRefQuoted,
                    r.DateCreation,
                    Statut = r.Statut.ToString()
                })
                .ToList();

            if (!rfqs.Any())
            {
                return NotFound($"No RFQs found with statut '{statutEnum}'.");
            }

            return Ok(rfqs);
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
            public Statut? Statut { get; set; }

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
        public int? NumRefQuoted { get; set; }
        public DateTime? SOPDate { get; set; }
        public int? MaxV { get; set; }
        public int? EstV { get; set; }
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
        public Statut? Statut { get; set; }
        public int? MaterialLeaderId { get; set; }
        public int? TestLeaderId { get; set; }
        public int? MarketSegmentId { get; set; }
        public int? ClientId { get; set; }
        public int? IngenieurRFQId { get; set; }
        public int? ValidateurId { get; set; }
    }

        public class RFQDetailsDto : RFQSummaryDto
    {
       
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
        public string MaterialLeader { get; set; }
        public string TestLeader { get; set; }
        public string MarketSegment { get; set; }
        public string IngenieurRFQ { get; set; }
        public string Validateur { get; set; }
        public string Client { get; set; }

    }





}
