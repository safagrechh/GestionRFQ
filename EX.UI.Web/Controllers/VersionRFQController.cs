using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionRFQController : ControllerBase
    {
        private readonly IService<VersionRFQ> _versionRFQService;
        private readonly IService<RFQ> _rfqService;

        public VersionRFQController(IService<VersionRFQ> versionRFQService, IService<RFQ> rfqService)
        {
            _versionRFQService = versionRFQService;
            _rfqService = rfqService;
        }

        // GET: api/VersionRFQ
        [HttpGet]
        public ActionResult<IEnumerable<VersionRFQSummaryDto>> GetAll()
        {
            var versionRFQs = _versionRFQService.GetAll();
            var versionRFQDtos = versionRFQs.Select(v => new VersionRFQSummaryDto
            {
                CodeV = v.CodeV,
                QuoteName = v.QuoteName,
                NumRefQuoted = v.NumRefQuoted,
                RFQId = v.RFQId
            }).ToList();

            return Ok(versionRFQDtos);
        }

        // GET: api/VersionRFQ/{id}
        [HttpGet("{id}")]
        public ActionResult<VersionRFQDetailsDto> Get(int id)
        {
            var versionRFQ = _versionRFQService.Get(id);
            if (versionRFQ == null)
            {
                return NotFound();
            }

            var versionRFQDto = new VersionRFQDetailsDto
            {
                CodeV = versionRFQ.CodeV,
                QuoteName = versionRFQ.QuoteName,
                NumRefQuoted = versionRFQ.NumRefQuoted,
                SOPDate = versionRFQ.SOPDate,
                MaxV = versionRFQ.MaxV,
                EstV = versionRFQ.EstV,
                KODate = versionRFQ.KODate,
                CustomerDataDate = versionRFQ.CustomerDataDate,
                MDDate = versionRFQ.MDDate,
                MRDate = versionRFQ.MRDate,
                TDDate = versionRFQ.TDDate,
                TRDate = versionRFQ.TRDate,
                LDDate = versionRFQ.LDDate,
                LRDate = versionRFQ.LRDate,
                CDDate = versionRFQ.CDDate,
                ApprovalDate = versionRFQ.ApprovalDate,
                DateCreation = versionRFQ.DateCreation,
                Statut = versionRFQ.Statut,
                RFQId = versionRFQ.RFQId,
                RFQQuoteName = versionRFQ.RFQ?.QuoteName,
                MaterialLeader = versionRFQ.MaterialLeader?.Nom,
                TestLeader = versionRFQ.TestLeader?.Nom,
                MarketSegment = versionRFQ.MarketSegment?.Nom,
                IngenieurRFQ = versionRFQ.IngenieurRFQ?.NomUser,
                Validateur = versionRFQ.Validateur?.NomUser
            };

            return Ok(versionRFQDto);
        }

        // POST: api/VersionRFQ
        [HttpPost]
        public ActionResult<VersionRFQ> Create([FromBody] CreateVersionRFQDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var originalRFQ = _rfqService.Get(dto.RFQId);
            if (originalRFQ == null)
            {
                return NotFound("Original RFQ not found.");
            }

            var versionRFQ = new VersionRFQ
            {
                QuoteName = dto.QuoteName ?? originalRFQ.QuoteName,
                NumRefQuoted = dto.NumRefQuoted ?? originalRFQ.NumRefQuoted,
                SOPDate = dto.SOPDate ?? originalRFQ.SOPDate,
                MaxV = dto.MaxV ?? originalRFQ.MaxV, // <-- Inherit MaxV
                EstV = dto.EstV ?? originalRFQ.EstV, // <-- Inherit EstV
                Statut = dto.Statut ?? originalRFQ.Statut,
                KODate = dto.KODate ?? originalRFQ.KODate,
                CustomerDataDate = dto.CustomerDataDate ?? originalRFQ.CustomerDataDate,
                MDDate = dto.MDDate ?? originalRFQ.MDDate,
                MRDate = dto.MRDate ?? originalRFQ.MRDate,
                TDDate = dto.TDDate ?? originalRFQ.TDDate,
                TRDate = dto.TRDate ?? originalRFQ.TRDate,
                LDDate = dto.LDDate ?? originalRFQ.LDDate,
                LRDate = dto.LRDate ?? originalRFQ.LRDate,
                CDDate = dto.CDDate ?? originalRFQ.CDDate,
                ApprovalDate = dto.ApprovalDate ,
                DateCreation = DateTime.UtcNow,
                RFQId = dto.RFQId,
                MaterialLeaderId = dto.MaterialLeaderId ?? originalRFQ.MaterialLeaderId,
                TestLeaderId = dto.TestLeaderId ?? originalRFQ.TestLeaderId,
                MarketSegmentId = dto.MarketSegmentId ?? originalRFQ.MarketSegmentId,
                IngenieurRFQId = dto.IngenieurRFQId ?? originalRFQ.IngenieurRFQId,
                ValidateurId = dto.ValidateurId ?? originalRFQ.ValidateurId
            };

            _versionRFQService.Add(versionRFQ);
            return CreatedAtAction(nameof(Get), new { id = versionRFQ.CodeV }, versionRFQ);
        }



        // PUT: api/VersionRFQ/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateVersionRFQDto dto)
        {
            if (id != dto.CodeV)
            {
                return BadRequest("VersionRFQ ID mismatch");
            }

            var existingVersionRFQ = _versionRFQService.Get(id);
            if (existingVersionRFQ == null)
            {
                return NotFound();
            }

            existingVersionRFQ.QuoteName = dto.QuoteName ?? existingVersionRFQ.QuoteName;
            existingVersionRFQ.NumRefQuoted = dto.NumRefQuoted ?? existingVersionRFQ.NumRefQuoted;
            existingVersionRFQ.SOPDate = dto.SOPDate ?? existingVersionRFQ.SOPDate;
            existingVersionRFQ.MaxV = dto.MaxV ?? existingVersionRFQ.MaxV;
            existingVersionRFQ.EstV = dto.EstV ?? existingVersionRFQ.EstV;
            existingVersionRFQ.Statut = dto.Statut ?? existingVersionRFQ.Statut;
            existingVersionRFQ.KODate = dto.KODate ?? existingVersionRFQ.KODate;
            existingVersionRFQ.CustomerDataDate = dto.CustomerDataDate  ?? existingVersionRFQ.CustomerDataDate;
            existingVersionRFQ.MDDate = dto.MDDate ?? existingVersionRFQ.MDDate;
            existingVersionRFQ.MRDate = dto.MRDate ?? existingVersionRFQ.MRDate;
            existingVersionRFQ.TDDate = dto.TDDate ?? existingVersionRFQ.TDDate;
            existingVersionRFQ.TRDate = dto.TRDate ?? existingVersionRFQ.TRDate;
            existingVersionRFQ.LDDate = dto.LDDate ?? existingVersionRFQ.LDDate;
            existingVersionRFQ.LRDate = dto.LRDate ?? existingVersionRFQ.LRDate;
            existingVersionRFQ.CDDate = dto.CDDate ?? existingVersionRFQ.CDDate;
            existingVersionRFQ.ApprovalDate = dto.ApprovalDate ?? existingVersionRFQ.ApprovalDate;
            existingVersionRFQ.MaterialLeaderId = dto.MaterialLeaderId ?? existingVersionRFQ.MaterialLeaderId;
            existingVersionRFQ.TestLeaderId = dto.TestLeaderId ?? existingVersionRFQ.TestLeaderId;
            existingVersionRFQ.MarketSegmentId = dto.MarketSegmentId ?? existingVersionRFQ.MarketSegmentId;
            existingVersionRFQ.ValidateurId = dto.ValidateurId ?? existingVersionRFQ.ValidateurId;
            existingVersionRFQ.IngenieurRFQId = dto.IngenieurRFQId ?? existingVersionRFQ.IngenieurRFQId;

            _versionRFQService.Update(existingVersionRFQ);

            return NoContent();
        }

        // DELETE: api/VersionRFQ/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var versionRFQ = _versionRFQService.Get(id);
            if (versionRFQ == null)
            {
                return NotFound();
            }

            _versionRFQService.Delete(versionRFQ);
            return NoContent();
        }
    }

    // DTOs for the VersionRFQ API
    public class CreateVersionRFQDto
    {
        public int RFQId { get; set; }
        public string QuoteName { get; set; }
        public int? NumRefQuoted { get; set; }
        public DateTime? SOPDate { get; set; }
        public int? MaxV { get; set; }
        public int? EstV { get; set; }
        public Statut? Statut { get; set; }
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
        public int? MaterialLeaderId { get; set; }
        public int? TestLeaderId { get; set; }
        public int? MarketSegmentId { get; set; }
        public int? IngenieurRFQId { get; set; }
        public int? ValidateurId { get; set; }
    }

    public class UpdateVersionRFQDto
    {
        public int CodeV { get; set; }
        public string QuoteName { get; set; }
        public int? NumRefQuoted { get; set; }
        public DateTime? SOPDate { get; set; }
        public int? MaxV { get; set; }
        public int? EstV { get; set; }
        public Statut? Statut { get; set; }
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
        public int? MaterialLeaderId { get; set; }
        public int? TestLeaderId { get; set; }
        public int? MarketSegmentId { get; set; }
        public int? IngenieurRFQId { get; set; }
        public int? ValidateurId { get; set; }
    }

    public class VersionRFQSummaryDto
    {
        public int CodeV { get; set; }
        public string QuoteName { get; set; }
        public int NumRefQuoted { get; set; }
        public int RFQId { get; set; }
    }

    public class VersionRFQDetailsDto : VersionRFQSummaryDto
    {
        public DateTime? SOPDate { get; set; }
        public int MaxV { get; set; }
        public int EstV { get; set; }
        public Statut Statut { get; set; }
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
        public string RFQQuoteName { get; set; }
        public string MaterialLeader { get; set; }
        public string TestLeader { get; set; }
        public string MarketSegment { get; set; }
        public string IngenieurRFQ { get; set; }
        public string Validateur { get; set; }
    }
}
