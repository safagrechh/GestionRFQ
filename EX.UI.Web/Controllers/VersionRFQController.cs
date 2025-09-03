using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionRFQController : ControllerBase
    {
        private readonly IService<VersionRFQ> _versionRFQService;
        private readonly IService<RFQ> _rfqService;
        private readonly INotificationService _notificationService;

        public VersionRFQController(IService<VersionRFQ> versionRFQService, IService<RFQ> rfqService, INotificationService notificationService)
        {
            _versionRFQService = versionRFQService;
            _rfqService = rfqService;
            _notificationService = notificationService;
        }

        // GET: api/VersionRFQ
        [HttpGet]
        public ActionResult<IEnumerable<VersionRFQSummaryDto>> GetAll()
        {
            var versionRFQs = _versionRFQService.GetAll();
            var versionRFQDtos = versionRFQs.Select(v => new VersionRFQSummaryDto
            {
                CQ = v.CQ,
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
                Id = versionRFQ.Id,
                CQ = versionRFQ.CQ,
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
                Valide = versionRFQ.Valide,
                Rejete = versionRFQ.Rejete,
                Statut = versionRFQ.Statut,
                RFQId = versionRFQ.RFQId,
                RFQQuoteName = versionRFQ.RFQ?.QuoteName,
                MaterialLeader = versionRFQ.MaterialLeader?.Nom,
                TestLeader = versionRFQ.TestLeader?.Nom,
                MarketSegment = versionRFQ.MarketSegment?.Nom,
                IngenieurRFQ = versionRFQ.IngenieurRFQ?.NomUser,
                VALeader = versionRFQ.VALeader?.NomUser,
                FileName = versionRFQ.FileName,
                FileContentType = versionRFQ.FileContentType,
                Client = versionRFQ.Client?.Nom,
            };

            return Ok(versionRFQDto);
        }

        // Add this endpoint to download the file
        [Authorize(Roles = "Validateur,IngenieurRFQ,Admin")]
        [HttpGet("{id}/file")]
        public IActionResult DownloadFile(int id)
        {
            var version = _versionRFQService.Get(id);
            if (version == null || version.FileData == null)
            {
                return NotFound();
            }

            return File(version.FileData, version.FileContentType, version.FileName);
        }


      
        // POST: api/VersionRFQ
        [HttpPost]
        public async Task<ActionResult<VersionRFQ>> Create([FromForm] CreateVersionRFQDto dto)
        {
            try
            {
                ModelState.Clear();

            var originalRFQ = _rfqService.Get(dto.RFQId);
            if (originalRFQ == null)
            {
                return NotFound("Original RFQ not found.");
            }

            var versionRFQ = new VersionRFQ
            {
                CQ = dto.CQ ?? originalRFQ.CQ,
                QuoteName = dto.QuoteName ?? originalRFQ.QuoteName,
                NumRefQuoted = dto.NumRefQuoted ?? originalRFQ.NumRefQuoted,
                SOPDate = dto.SOPDate ?? originalRFQ.SOPDate,
                MaxV = dto.MaxV ?? originalRFQ.MaxV,
                EstV = dto.EstV ?? originalRFQ.EstV,
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
                ApprovalDate = dto.ApprovalDate,
                DateCreation = DateTime.UtcNow,
                RFQId = dto.RFQId,
                MaterialLeaderId = dto.MaterialLeaderId ?? originalRFQ.MaterialLeaderId,
                TestLeaderId = dto.TestLeaderId ?? originalRFQ.TestLeaderId,
                MarketSegmentId = dto.MarketSegmentId ?? originalRFQ.MarketSegmentId,
                IngenieurRFQId = dto.IngenieurRFQId ?? originalRFQ.IngenieurRFQId,
                VALeaderId = dto.VALeaderId ?? originalRFQ.VALeaderId,
                ClientId = dto.ClientId ?? originalRFQ.ClientId,
                Valide = dto.Valide ?? originalRFQ.Valide,
                Rejete = dto.Rejete ?? originalRFQ.Rejete,
            };

            // Handle file upload - use new file if provided, otherwise inherit from RFQ if available
            if (dto.File != null && dto.File.Length > 0)
            {
                await HandleFileUpload(dto.File, versionRFQ);
            }
            else if (originalRFQ.FileData != null)
            {
                versionRFQ.FileData = originalRFQ.FileData;
                versionRFQ.FileName = originalRFQ.FileName;
                versionRFQ.FileContentType = originalRFQ.FileContentType;
            }

            _versionRFQService.Add(versionRFQ);
            
            // Get the current user's ID and name from JWT claims
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var actionUserName = User.FindFirst("name")?.Value ?? 
                                   User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                   "Utilisateur inconnu";

                // Check if current user is a Validateur
                bool isValidateur = User.IsInRole("Validateur");

                if (isValidateur)
                {
                    // If Validateur creates version, notify only the assigned engineer
                    if (versionRFQ.IngenieurRFQId.HasValue)
                    {
                        var engineerMessage = $"Une nouvelle version a été créée pour la RFQ '{versionRFQ.QuoteName}' (CQ: {versionRFQ.CQ}) qui vous est assignée par {actionUserName}.";
                        await _notificationService.CreateNotification(engineerMessage, versionRFQ.IngenieurRFQId.Value, versionRFQ.RFQId, actionUserName);
                    }
                }
                else
                {
                    // If Engineer creates version, notify all Validateurs
                    var validateurMessage = $"Nouvelle version créée pour la RFQ '{versionRFQ.QuoteName}' (CQ: {versionRFQ.CQ}) par {actionUserName}.";
                    await _notificationService.CreateNotificationsForRole(validateurMessage, "Validateur", versionRFQ.RFQId, actionUserName);
                }
            
            return CreatedAtAction(nameof(Get), new { id = versionRFQ.Id }, versionRFQ);

            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new
                {
                    Title = "Server Error",
                    Message = ex.Message,
                    Details = ex.StackTrace
                });
            }
        }


        // Add this helper method to handle file uploads
        private async Task HandleFileUpload(IFormFile file, VersionRFQ versionRFQ)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                versionRFQ.FileData = memoryStream.ToArray();
                versionRFQ.FileName = file.FileName;
                versionRFQ.FileContentType = file.ContentType;
            }
        }

        // PUT: api/VersionRFQ/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<VersionRFQ>> Update(int id, [FromForm] UpdateVersionRFQDto dto)
        {
            var existingVersionRFQ = _versionRFQService.Get(id);
            if (existingVersionRFQ == null)
            {
                return NotFound();
            }

            existingVersionRFQ.CQ = dto.CQ ?? existingVersionRFQ.CQ;
            existingVersionRFQ.QuoteName = dto.QuoteName ?? existingVersionRFQ.QuoteName;
            existingVersionRFQ.NumRefQuoted = dto.NumRefQuoted ?? existingVersionRFQ.NumRefQuoted;
            existingVersionRFQ.SOPDate = dto.SOPDate ?? existingVersionRFQ.SOPDate;
            existingVersionRFQ.MaxV = dto.MaxV ?? existingVersionRFQ.MaxV;
            existingVersionRFQ.EstV = dto.EstV ?? existingVersionRFQ.EstV;
            existingVersionRFQ.KODate = dto.KODate ?? existingVersionRFQ.KODate;
            existingVersionRFQ.CustomerDataDate = dto.CustomerDataDate ?? existingVersionRFQ.CustomerDataDate;
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
            existingVersionRFQ.VALeaderId = dto.VALeaderId ?? existingVersionRFQ.VALeaderId;
            existingVersionRFQ.ClientId = dto.ClientId ?? existingVersionRFQ.ClientId;
            existingVersionRFQ.Valide = dto.Valide ?? existingVersionRFQ.Valide;
            existingVersionRFQ.Rejete = false;

            // Handle file upload - only update if new file is provided, otherwise keep existing file
            if (dto.File != null && dto.File.Length > 0)
            {
                await HandleFileUpload(dto.File, existingVersionRFQ);
            }

            _versionRFQService.Update(existingVersionRFQ);
            
            // Get the current user's ID and name from JWT claims
              var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
              var actionUserName = User.FindFirst("name")?.Value ?? 
                                 User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                 User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                 "Utilisateur inconnu";
 
              // Check if current user is a Validateur
              bool isValidateur = User.IsInRole("Validateur");

              if (isValidateur)
              {
                  // If Validateur updates version, notify only the assigned engineer
                  if (existingVersionRFQ.IngenieurRFQId.HasValue)
                  {
                      var engineerMessage = $"La version RFQ '{existingVersionRFQ.QuoteName}' (CQ: {existingVersionRFQ.CQ}) qui vous est assignée a été mise à jour par {actionUserName}.";
                      await _notificationService.CreateNotification(engineerMessage, existingVersionRFQ.IngenieurRFQId.Value, existingVersionRFQ.RFQId, actionUserName);
                  }
              }
              else
              {
                  // If Engineer updates version, notify all Validateurs
                  var validateurMessage = $"Version RFQ '{existingVersionRFQ.QuoteName}' (CQ: {existingVersionRFQ.CQ}) mise à jour par {actionUserName}.";
                  await _notificationService.CreateNotificationsForRole(validateurMessage, "Validateur", existingVersionRFQ.RFQId, actionUserName);
              }
            
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
        // GET: api/VersionRFQ/by-rfq/{rfqId}
        [HttpGet("by-rfq/{rfqId}")]
        public ActionResult<IEnumerable<VersionRFQSummaryDto>> GetByRFQId(int rfqId)
        {
            var versionRFQs = _versionRFQService.GetAll().Where(v => v.RFQId == rfqId).ToList();

            if (!versionRFQs.Any())
            {
                return NotFound($"No versions found for RFQ ID {rfqId}.");
            }

            var versionRFQDtos = versionRFQs.Select(v => new VersionRFQSummaryDto
            {   Id = v.Id,
                CQ = v.CQ,
                QuoteName = v.QuoteName,
                NumRefQuoted = v.NumRefQuoted,
                RFQId = v.RFQId,
                Valide = v.Valide,
                Rejete = v.Rejete
            }).ToList();

            return Ok(versionRFQDtos);
        }

        [Authorize(Roles = "Validateur")]
        [HttpPost("{id}/valider")]
        public async Task<IActionResult> Valider(int id)
        {
            var rfq = _versionRFQService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            rfq.Valide = true;
            rfq.ApprovalDate = DateTime.UtcNow;
            _versionRFQService.Update(rfq);

            // Create notification for the RFQ engineer if assigned
            if (rfq.IngenieurRFQId.HasValue)
            {
                var message = $"Version RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) a été validée.";
                
                // Get the current user's name from JWT claims
                var actionUserName = User.FindFirst("name")?.Value ?? 
                                   User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                   "Utilisateur inconnu";

                await _notificationService.CreateNotification(message, rfq.IngenieurRFQId.Value, rfq.RFQId, actionUserName);
            }

            return Ok(rfq);
        }

        [Authorize(Roles = "Validateur")]
        [HttpPost("{id}/rejeter")]
        public async Task<IActionResult> Rejeter(int id)
        {
            var rfq = _versionRFQService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            rfq.Rejete = true;
            _versionRFQService.Update(rfq);

            // Create notification for the RFQ engineer if assigned
            if (rfq.IngenieurRFQId.HasValue)
            {
                var message = $"Version RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) a été rejetée.";
                
                // Get the current user's name from JWT claims
                var actionUserName = User.FindFirst("name")?.Value ?? 
                                   User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                   "Utilisateur inconnu";

                await _notificationService.CreateNotification(message, rfq.IngenieurRFQId.Value, rfq.RFQId, actionUserName);
            }

            return Ok(rfq);
        }

    }

    // DTOs for the VersionRFQ API
    public class CreateVersionRFQDto
    {
        public int RFQId { get; set; }
        public int? CQ { get; set; }
        public string? QuoteName { get; set; }
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
        public int? VALeaderId { get; set; }
        public int? ClientId { get; set; }

        public Boolean? Valide { get; set; }
        public Boolean? Rejete { get; set; }

        public IFormFile? File { get; set; }
        
    }

    public class UpdateVersionRFQDto
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
        public int? MaterialLeaderId { get; set; }
        public int? TestLeaderId { get; set; }
        public int? MarketSegmentId { get; set; }
        public int? IngenieurRFQId { get; set; }
        public int? VALeaderId { get; set; }
        public int? ClientId { get; set; }


        public Boolean? Valide { get; set; }
        public Boolean? Rejete { get; set; }
        

        public IFormFile? File { get; set; }

    }

    public class VersionRFQSummaryDto
    {
        public int Id { get; set; }

        public int? CQ { get; set; }
        public string QuoteName { get; set; }
        public int? NumRefQuoted { get; set; }
        public int RFQId { get; set; }
        public Boolean Valide { get; set; }
        public Boolean Rejete { get; set; }
    }

    public class VersionRFQDetailsDto : VersionRFQSummaryDto
    {
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
        public DateTime DateCreation { get; set; }
        public string RFQQuoteName { get; set; }
        public string MaterialLeader { get; set; }
        public string TestLeader { get; set; }
        public string MarketSegment { get; set; }
        public string IngenieurRFQ { get; set; }
        public string VALeader { get; set; }

        public string Client { get; set; }

        public string? FileName { get; set; }
        public string? FileContentType { get; set; }
        public byte[]? FileData { get; set; }

    }
}
