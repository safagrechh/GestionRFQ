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
            {   Id = r.Id , 
                CQ = r.CQ,
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
                VALeader = r.VALeader?.NomUser,
                Client = r.Client?.Nom ,
                Valide = r.Valide ,
                Rejete = r.Rejete ,
                Brouillon = r.Brouillon,
                    
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
                CQ = rfq.CQ,
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
                VALeader = rfq.VALeader?.NomUser,
                Client = rfq.Client?.Nom ,
                Valide = rfq.Valide ,
                Rejete = rfq.Rejete ,
                Brouillon = rfq.Brouillon ,

            };

            return Ok(rfqDto);
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPost]
        public ActionResult<RFQ> Create([FromBody] CreateRFQDto dto)
        {
            // Skip validation if Brouillon is true
            if (dto.Brouillon == true)
            {
                ModelState.Clear();
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rfq = new RFQ
            {   CQ = dto.CQ,
                QuoteName = dto.QuoteName,
                NumRefQuoted = dto.NumRefQuoted,
                SOPDate = dto.SOPDate,
                MaxV = dto.MaxV,
                EstV = dto.EstV,
                Statut = dto.Statut ?? Statut.NotStarted, // Default to NotStarted
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
                VALeaderId = dto.VALeaderId,
                Valide = false,
                Rejete = false,
                Brouillon = dto.Brouillon,
            };

            _rfqService.Add(rfq);

            return CreatedAtAction(nameof(Get), new { id = rfq.Id }, rfq);
        }


        [Authorize(Roles = "Validateur")]
        [HttpPost("create-valide")]

        public ActionResult<RFQ> CreateValide([FromBody] CreateRFQDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rfq = new RFQ
            {
                CQ = dto.CQ,
                QuoteName = dto.QuoteName,
                NumRefQuoted = dto.NumRefQuoted,
                SOPDate = dto.SOPDate,
                MaxV = dto.MaxV,
                EstV = dto.EstV,
                Statut = dto.Statut ?? Statut.NotStarted, // Default to Brouillon
                KODate = dto.KODate,
                CustomerDataDate = dto.CustomerDataDate,
                MDDate = dto.MDDate,
                MRDate = dto.MRDate,
                TDDate = dto.TDDate,
                TRDate = dto.TRDate,
                LDDate = dto.LDDate,
                LRDate = dto.LRDate,
                CDDate = dto.CDDate,
                ApprovalDate = DateTime.UtcNow,
                DateCreation = DateTime.UtcNow,
                MaterialLeaderId = dto.MaterialLeaderId,
                TestLeaderId = dto.TestLeaderId,
                MarketSegmentId = dto.MarketSegmentId,
                ClientId = dto.ClientId,
                IngenieurRFQId = dto.IngenieurRFQId,
                VALeaderId = dto.VALeaderId,
                Valide = true,
                Rejete = false,
                Brouillon = false , 
            };

            _rfqService.Add(rfq);

            return CreatedAtAction(nameof(Get), new { id = rfq.Id }, rfq);
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPut("{id}")]
        public ActionResult<RFQ> Update(int id, [FromBody] UpdateRFQDto dto)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            // Skip validation if Brouillon is true
            if (dto.Brouillon == true)
            {
                ModelState.ClearValidationState(nameof(UpdateRFQDto));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            rfq.CQ = dto.CQ ?? rfq.CQ;
            rfq.QuoteName = dto.QuoteName ?? rfq.QuoteName;
            rfq.NumRefQuoted = dto.NumRefQuoted ?? rfq.NumRefQuoted;
            rfq.SOPDate = dto.SOPDate ?? rfq.SOPDate;
            rfq.MaxV = dto.MaxV ?? rfq.MaxV;
            rfq.EstV = dto.EstV ?? rfq.EstV;
            rfq.KODate = dto.KODate ?? rfq.KODate;
            rfq.CustomerDataDate = dto.CustomerDataDate ?? rfq.CustomerDataDate;
            rfq.MDDate = dto.MDDate ?? rfq.MDDate;
            rfq.MRDate = dto.MRDate ?? rfq.MRDate;
            rfq.TDDate = dto.TDDate ?? rfq.TDDate;
            rfq.TRDate = dto.TRDate ?? rfq.TRDate;
            rfq.LDDate = dto.LDDate ?? rfq.LDDate;
            rfq.LRDate = dto.LRDate ?? rfq.LRDate;
            rfq.CDDate = dto.CDDate ?? rfq.CDDate;
            rfq.ApprovalDate = dto.ApprovalDate ?? rfq.ApprovalDate;
            rfq.Statut = dto.Statut ?? rfq.Statut;
            rfq.MaterialLeaderId = dto.MaterialLeaderId ?? rfq.MaterialLeaderId;
            rfq.TestLeaderId = dto.TestLeaderId ?? rfq.TestLeaderId;
            rfq.MarketSegmentId = dto.MarketSegmentId ?? rfq.MarketSegmentId;
            rfq.ClientId = dto.ClientId ?? rfq.ClientId;
            rfq.IngenieurRFQId = dto.IngenieurRFQId ?? rfq.IngenieurRFQId;
            rfq.VALeaderId = dto.VALeaderId ?? rfq.VALeaderId;
            rfq.Valide = dto.Valide ?? rfq.Valide;
            rfq.Rejete = dto.Rejete ?? rfq.Rejete;
            rfq.Brouillon = dto.Brouillon ?? rfq.Brouillon;

            _rfqService.Update(rfq);

            return Ok(rfq);
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
        [HttpPut("{id}/finaliser")]
         public IActionResult FinaliserBrouillon(int id, [FromBody] UpdateRFQDto dto)
         {
             var rfq = _rfqService.Get(id);
             if (rfq == null)
             {
                 return NotFound();
             }

             rfq.CQ = dto.CQ ?? rfq.CQ;
             rfq.QuoteName = dto.QuoteName ?? rfq.QuoteName;
             rfq.NumRefQuoted = dto.NumRefQuoted ?? rfq.NumRefQuoted;
             rfq.SOPDate = dto.SOPDate ?? rfq.SOPDate;
             rfq.MaxV = dto.MaxV ?? rfq.MaxV;
             rfq.EstV = dto.EstV ?? rfq.EstV;
             rfq.KODate = dto.KODate ?? rfq.KODate;
             rfq.CustomerDataDate = dto.CustomerDataDate ?? rfq.CustomerDataDate;
             rfq.MDDate = dto.MDDate ?? rfq.MDDate;
             rfq.MRDate = dto.MRDate ?? rfq.MRDate;
             rfq.TDDate = dto.TDDate ?? rfq.TDDate;
             rfq.TRDate = dto.TRDate ?? rfq.TRDate;
             rfq.LDDate = dto.LDDate ?? rfq.LDDate;
             rfq.LRDate = dto.LRDate ?? rfq.LRDate;
             rfq.CDDate = dto.CDDate ?? rfq.CDDate;
             rfq.ApprovalDate = dto.ApprovalDate ?? rfq.ApprovalDate;
             rfq.Statut = dto.Statut ?? rfq.Statut;
             rfq.MaterialLeaderId = dto.MaterialLeaderId ?? rfq.MaterialLeaderId;
             rfq.TestLeaderId = dto.TestLeaderId ?? rfq.TestLeaderId;
             rfq.MarketSegmentId = dto.MarketSegmentId ?? rfq.MarketSegmentId;
             rfq.ClientId = dto.ClientId ?? rfq.ClientId;
             rfq.IngenieurRFQId = dto.IngenieurRFQId ?? rfq.IngenieurRFQId;
             rfq.VALeaderId = dto.VALeaderId ?? rfq.VALeaderId;
             rfq.Valide = false;
             rfq.Rejete = false;
             rfq.Brouillon = false;

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

            rfq.Valide = true ;
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


            rfq.Rejete = true;
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
                    r.CQ,
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
        public int? CQ { get; set; }
        public string QuoteName { get; set; }
        public int? NumRefQuoted { get; set; }  // Nullable for empty fields
        public DateTime? SOPDate { get; set; }  // Nullable for empty fields
        public int? MaxV { get; set; }  // Nullable for empty fields
        public int? EstV { get; set; }  // Nullable for empty fields
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
        public int? VALeaderId { get; set; }
        public Boolean Brouillon { get; set; }
    }


    public class UpdateRFQDto
    {
        public int? CQ { get; set; }
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
        public int? VALeaderId { get; set; }
        public Boolean? Valide { get; set; }
        public Boolean? Rejete { get; set; }
        public Boolean? Brouillon { get; set; }


    }

    public class RFQDetailsDto : RFQSummaryDto
    {
        public int Id { get; set; }
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
       
        public DateTime DateCreation { get; set; }
        public Statut Statut { get; set; }
        public string MaterialLeader { get; set; }
        public string TestLeader { get; set; }
        public string MarketSegment { get; set; }
        public string IngenieurRFQ { get; set; }
        public string VALeader { get; set; }
        public string Client { get; set; }

        public Boolean Valide { get; set; }
        public Boolean Rejete { get; set; }

        public Boolean Brouillon { get; set; }


    }





}
