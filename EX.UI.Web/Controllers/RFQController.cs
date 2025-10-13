using EX.Core.Domain;
using EX.Core.Services;
using EX.UI.Web.Hubs;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace EX.UI.Web.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class RFQController : ControllerBase
    {
        private readonly IService<RFQ> _rfqService;
        private readonly INotificationService _notificationService;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hub;

        public RFQController(IService<RFQ> rfqService , INotificationService notificationService,
        IEmailService emailService, IHubContext<NotificationHub> hub)
        {
            _rfqService = rfqService;
            _notificationService = notificationService;
            _emailService = emailService;
            _hub = hub;
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ,Admin")]
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
                FileName = r.FileName,
                FileContentType = r.FileContentType,
                VersionsCount = r.Versions?.Count ?? 0, // Changed to return count instead of versions
                CreatedByUserId = r.CreatedByUserId,
                CreatedByUser = r.CreatedByUser?.NomUser


            }).ToList();

            return Ok(rfqDtos);
        }


        [Authorize(Roles = "Validateur,IngenieurRFQ,Admin")]
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
                FileName = rfq.FileName,
                FileContentType = rfq.FileContentType,
                VersionsCount = rfq.Versions?.Count ?? 0, // Changed to return count instead of versions
                CreatedByUserId = rfq.CreatedByUserId,
                CreatedByUser = rfq.CreatedByUser?.NomUser

            };

            return Ok(rfqDto);
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPost]
        public async Task<ActionResult<RFQ>> Create([FromForm] CreateRFQDto dto)
        {
            try
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
                Statut = null,
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
            // Handle file upload
            if (dto.File != null && dto.File.Length > 0)
            {
                await HandleFileUpload(dto.File, rfq);
            }

            _rfqService.Add(rfq);

                // Get the current user's ID and name from JWT claims
                var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var actionUserName = User.FindFirst("name")?.Value ?? 
                                   User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                   "Utilisateur inconnu";

                // Always notify all Validateurs for every RFQ creation
                var validateurMessage = $"Nouvelle RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) créée par {actionUserName}.";
                await _notificationService.CreateNotificationsForRole(validateurMessage, "Validateur", rfq.Id, actionUserName);
                
                // Send email to all Validateurs
                var validateurEmailSubject = "Nouvelle RFQ créée";
                var validateurEmailBody = $"<h3>Nouvelle RFQ créée</h3><p>Une nouvelle RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) a été créée par {actionUserName}.</p><p>Veuillez vous connecter au système pour plus de détails.</p>";
                await _emailService.SendEmailToRoleAsync("Validateur", validateurEmailSubject, validateurEmailBody);

                // Also notify the assigned engineer if there is one and it's not the current user
                if (rfq.IngenieurRFQId.HasValue && int.TryParse(currentUserIdClaim, out int currentUserId) && currentUserId != rfq.IngenieurRFQId.Value)
                {
                    var engineerMessage = $"Nouvelle RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) vous a été assignée.";
                    await _notificationService.CreateNotification(engineerMessage, rfq.IngenieurRFQId.Value, rfq.Id, actionUserName);
                    
                    // Send email to assigned engineer
                    var engineerEmailSubject = "Nouvelle RFQ assignée";
                    var engineerEmailBody = $"<h3>Nouvelle RFQ assignée</h3><p>Une nouvelle RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) vous a été assignée.</p><p>Veuillez vous connecter au système pour plus de détails.</p>";
                    await _emailService.SendEmailToUserAsync(rfq.IngenieurRFQId.Value, engineerEmailSubject, engineerEmailBody);
                }

                return CreatedAtAction(nameof(Get), new { id = rfq.Id }, rfq);
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
        private async Task HandleFileUpload(IFormFile file, RFQ rfq)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                rfq.FileData = memoryStream.ToArray();
                rfq.FileName = file.FileName;
                rfq.FileContentType = file.ContentType;
            }
        }

        // Add this endpoint to download the file
        [Authorize(Roles = "Validateur,IngenieurRFQ,Admin")]
        [HttpGet("{id}/file")]
        public IActionResult DownloadFile(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null || rfq.FileData == null)
            {
                return NotFound();
            }

            return File(rfq.FileData, rfq.FileContentType, rfq.FileName);
        }


        [Authorize(Roles = "Validateur")]
        [HttpPost("create-valide")]

        public async Task<ActionResult<RFQ>> CreateValide([FromForm] CreateRFQDto dto)
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
                Statut = null, 
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
            // Handle file upload
            if (dto.File != null && dto.File.Length > 0)
            {
                await HandleFileUpload(dto.File, rfq);
            }

            _rfqService.Add(rfq);

            return CreatedAtAction(nameof(Get), new { id = rfq.Id }, rfq);
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPut("{id}")]
        public async Task<ActionResult<RFQ>> Update(int id, [FromForm] UpdateRFQDto dto)
        {   if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            

            // Update properties
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
            rfq.Rejete = false;
            rfq.Brouillon = dto.Brouillon ?? rfq.Brouillon;

            // Handle file upload if a new file is provided
            if (dto.File != null && dto.File.Length > 0)
            {
                await HandleFileUpload(dto.File, rfq);
            }

            _rfqService.Update(rfq);

            // Get the current user's ID and name from JWT claims
             var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             var actionUserName = User.FindFirst("name")?.Value ?? 
                                User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                "Utilisateur inconnu";

             // Get current user info
             int.TryParse(currentUserIdClaim, out int currentUserId);
             var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

             // If current user is a Validateur, notify the assigned engineer (if exists and different from current user)
             if (currentUserRole == "Validateur" && rfq.IngenieurRFQId.HasValue && currentUserId != rfq.IngenieurRFQId.Value)
             {
                 var engineerMessage = $"La RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) qui vous est assignée a été mise à jour par {actionUserName}.";
                 await _notificationService.CreateNotification(engineerMessage, rfq.IngenieurRFQId.Value, rfq.Id, actionUserName);
                 
                 // Send email to assigned engineer
                 var engineerEmailSubject = "RFQ assignée mise à jour";
                 var engineerEmailBody = $"<h3>RFQ mise à jour</h3><p>La RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) qui vous est assignée a été mise à jour par {actionUserName}.</p><p>Veuillez vous connecter au système pour plus de détails.</p>";
                 await _emailService.SendEmailToUserAsync(rfq.IngenieurRFQId.Value, engineerEmailSubject, engineerEmailBody);
             }
             // If current user is an Engineer, notify all Validateurs
             else if (currentUserRole == "IngenieurRFQ")
             {
                 var validateurMessage = $"RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) mise à jour par {actionUserName}.";
                 await _notificationService.CreateNotificationsForRole(validateurMessage, "Validateur", rfq.Id, actionUserName);
                 
                 // Send email to all Validateurs
                 var validateurEmailSubject = "RFQ mise à jour";
                 var validateurEmailBody = $"<h3>RFQ mise à jour</h3><p>La RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) a été mise à jour par {actionUserName}.</p><p>Veuillez vous connecter au système pour plus de détails.</p>";
                 await _emailService.SendEmailToRoleAsync("Validateur", validateurEmailSubject, validateurEmailBody);
             }

            // Return a proper response
            return Ok(new
            {
                success = true,
                message = "RFQ updated successfully",
                data = rfq
            });
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPut("{id}/update-statut")]
        public IActionResult UpdateStatut(int id, [FromBody] UpdateStatutDto dto)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            // Validate the statut value
            if (!Enum.IsDefined(typeof(Statut), dto.Statut))
            {
                return BadRequest($"Invalid statut value. Valid values are: {string.Join(", ", Enum.GetNames(typeof(Statut)))}");
            }

            // Only update the Statut field
            rfq.Statut = dto.Statut;

            _rfqService.Update(rfq);

            return Ok(new
            {
                success = true,
                message = "Statut updated successfully",
                data = new
                {
                    id = rfq.Id,
                    newStatut = rfq.Statut.ToString()
                }
            });
        }

        // DTO for updating only the Statut
        public class UpdateStatutDto
        {
            public Statut Statut { get; set; }
        }

        [Authorize(Roles = "Validateur,IngenieurRFQ")]
        [HttpPost("draft")]
        public async Task<ActionResult<RFQ>> CreateDraft([FromForm] CreateRFQDraftDto dto)
        {
            // No validation - all fields are optional for drafts
            var rfq = new RFQ
            {
                CQ = dto.CQ,
                QuoteName = dto.QuoteName ?? "DRAFT-" + Guid.NewGuid().ToString()[..8],
                NumRefQuoted = dto.NumRefQuoted,
                SOPDate = dto.SOPDate,
                MaxV = dto.MaxV,
                EstV = dto.EstV,
                Statut = null, // Default for drafts
                KODate = dto.KODate,
                CustomerDataDate = dto.CustomerDataDate,
                MDDate = dto.MDDate,
                MRDate = dto.MRDate,
                TDDate = dto.TDDate,
                TRDate = dto.TRDate,
                LDDate = dto.LDDate,
                LRDate = dto.LRDate,
                CDDate = dto.CDDate,
                ApprovalDate = null,
                DateCreation = DateTime.UtcNow,
                MaterialLeaderId = dto.MaterialLeaderId,
                TestLeaderId = dto.TestLeaderId,
                MarketSegmentId = dto.MarketSegmentId,
                ClientId = dto.ClientId,
                IngenieurRFQId = dto.IngenieurRFQId,
                VALeaderId = dto.VALeaderId,
                Valide = false,
                Rejete = false,
                Brouillon = true
            };

            if (dto.File != null && dto.File.Length > 0)
            {
                await HandleFileUpload(dto.File, rfq);
            }

            _rfqService.Add(rfq);
            return CreatedAtAction(nameof(Get), new { id = rfq.Id }, rfq);
        }

        [Authorize(Roles = "Admin")]
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
        [HttpPut("{id}/valider")]
        public async Task<IActionResult> Valider(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            rfq.Valide = true;
            rfq.ApprovalDate = DateTime.UtcNow;
            _rfqService.Update(rfq);

            // Create notification for the RFQ engineer if assigned
            if (rfq.IngenieurRFQId.HasValue)
            {
                var message = $"RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) a été validée.";
                
                // Get the current user's name from JWT claims
                var actionUserName = User.FindFirst("name")?.Value ?? 
                                   User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                   "Utilisateur inconnu";

                await _notificationService.CreateNotification(message, rfq.IngenieurRFQId.Value, rfq.Id, actionUserName);
            }

            return Ok(rfq);
        }

        [Authorize(Roles = "Validateur")]
        [HttpPut("{id}/rejeter")]
        public async Task<IActionResult> Rejeter(int id)
        {
            var rfq = _rfqService.Get(id);
            if (rfq == null)
            {
                return NotFound();
            }

            rfq.Rejete = true;
            _rfqService.Update(rfq);

            // Create notification for the RFQ engineer if assigned
            if (rfq.IngenieurRFQId.HasValue)
            {
                var message = $"RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ}) a été rejetée.";
                
                // Get the current user's name from JWT claims
                var actionUserName = User.FindFirst("name")?.Value ?? 
                                   User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                   "Utilisateur inconnu";

                await _notificationService.CreateNotification(message, rfq.IngenieurRFQId.Value, rfq.Id, actionUserName);
            }

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

        [Authorize(Roles = "Validateur,IngenieurRFQ,Admin")]
        [HttpGet("drafts/{userId}")]
        public ActionResult<IEnumerable<RFQDetailsDto>> GetUserDrafts(int userId)
        {
            try
            {
                var drafts = _rfqService.GetAll()
                    .Where(r => r.Brouillon == true &&  r.CreatedByUserId == userId)
                    .Select(r => new RFQDetailsDto
                    {
                        Id = r.Id,
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
                        Client = r.Client?.Nom,
                        Valide = r.Valide,
                        Rejete = r.Rejete,
                        Brouillon = r.Brouillon,
                        FileName = r.FileName,
                        FileContentType = r.FileContentType,
                        VersionsCount = r.Versions?.Count ?? 0,
                        CreatedByUserId = r.CreatedByUserId,
                        CreatedByUser = r.CreatedByUser?.NomUser
                    })
                    .ToList();

                return Ok(drafts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Title = "Server Error",
                    Message = "An error occurred while retrieving user drafts.",
                    Details = ex.Message
                });
            }
        }

    }
    public class CreateRFQDto
    {
        public int CQ { get; set; }
        public string? QuoteName { get; set; }
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
        public int ClientId { get; set; }
        public int? IngenieurRFQId { get; set; }
        public int? VALeaderId { get; set; }
        public IFormFile? File { get; set; }
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
        public IFormFile? File { get; set; }
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
        public Statut? Statut { get; set; }
        public string MaterialLeader { get; set; }
        public string TestLeader { get; set; }
        public string MarketSegment { get; set; }
        public string IngenieurRFQ { get; set; }
        public string VALeader { get; set; }
        public string Client { get; set; }

        public Boolean Valide { get; set; }
        public Boolean Rejete { get; set; }

        public Boolean Brouillon { get; set; }

        public string? FileName { get; set; }
        public string? FileContentType { get; set; }
        public byte[]? FileData { get; set; }

        public int VersionsCount { get; set; }

        // CreatedByUser fields
        public int? CreatedByUserId { get; set; }
        public string? CreatedByUser { get; set; }

    }


    public class CreateRFQDraftDto
    {
        public int CQ { get; set; }
        public string? QuoteName { get; set; }
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
        public int? MaterialLeaderId { get; set; }
        public int? TestLeaderId { get; set; }
        public int? MarketSegmentId { get; set; }
        public int ClientId { get; set; }
        public int? IngenieurRFQId { get; set; }
        public int? VALeaderId { get; set; }
        public IFormFile? File { get; set; }
    }


}
