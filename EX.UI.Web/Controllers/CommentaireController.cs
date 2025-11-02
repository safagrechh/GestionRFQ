using EX.Core.Domain;
using EX.Core.Services;
using EX.UI.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class CommentaireController : ControllerBase
    {
        private readonly IService<Commentaire> _commentaireService;
        private readonly IService<RFQ> _rfqService;
        private readonly IService<VersionRFQ> _versionRFQService;
        private readonly IService<User> _userService;
        private readonly INotificationService _notificationService;
        private readonly IActionHistoryLogger _actionHistoryLogger;

        public CommentaireController(
            IService<Commentaire> commentaireService,
            IService<User> userService,
            IService<RFQ> rfqService,
            IService<VersionRFQ> versionRFQService,
            INotificationService notificationService,
            IActionHistoryLogger actionHistoryLogger)
        {
            _commentaireService = commentaireService;
            _userService = userService;
            _rfqService = rfqService;
            _versionRFQService = versionRFQService;
            _notificationService = notificationService;
            _actionHistoryLogger = actionHistoryLogger;
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out userId))
            {
                return true;
            }

            userId = 0;
            return false;
        }

        [HttpPost]
        [Authorize(Roles = "Validateur")]
        public async Task<ActionResult<Commentaire>> Create([FromBody] CreateCommentaireDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryGetCurrentUserId(out int validateurId))
            {
                return Unauthorized(new { message = "Validateur ID not found in token" });
            }

            // Validate RFQ/VersionRFQ input
            if (dto.RFQId.HasValue && dto.VersionRFQId.HasValue)
            {
                return BadRequest("A comment can only be associated with either an RFQ or a VersionRFQ, not both.");
            }

            if (!dto.RFQId.HasValue && !dto.VersionRFQId.HasValue)
            {
                return BadRequest("A comment must be associated with either an RFQ or a VersionRFQ.");
            }

            // Validate RFQ or VersionRFQ existence
            RFQ rfq = null;
            VersionRFQ versionRFQ = null;

            if (dto.RFQId.HasValue)
            {
                rfq = _rfqService.Get(dto.RFQId.Value);
                if (rfq == null)
                {
                    return NotFound("RFQ not found.");
                }
            }

            if (dto.VersionRFQId.HasValue)
            {
                versionRFQ = _versionRFQService.Get(dto.VersionRFQId.Value);
                if (versionRFQ == null)
                {
                    return NotFound("Version RFQ not found.");
                }
            }

            // Create Commentaire entity
            var commentaire = new Commentaire
            {
                Contenu = dto.Contenu,
                DateC = DateTime.UtcNow,
                ValidateurId = validateurId,
                RFQId = dto.RFQId,
                VersionRFQId = dto.VersionRFQId
            };

            _commentaireService.Add(commentaire);

            var cibleAction = dto.RFQId.HasValue ? "COMMENTAIRE_RFQ" : "COMMENTAIRE_VERSION";
            var referenceCible = dto.RFQId.HasValue ? rfq?.CQ.ToString() : versionRFQ?.CQ.ToString();
            var entityDescription = dto.RFQId.HasValue
                ? $"RFQ '{rfq?.QuoteName}' (CQ: {rfq?.CQ})"
                : $"Version RFQ '{versionRFQ?.QuoteName}' (CQ: {versionRFQ?.CQ})";

            _actionHistoryLogger.LogAction(
                "COMMENT_CREATED",
                cibleAction,
                referenceCible,
                $"Commentaire créé sur {entityDescription}.",
                validateurId);

            // Create notification for the RFQ engineer if assigned
            int? engineerId = null;
            int rfqId = 0;
            string entityName = "";
            
            if (rfq != null)
            {
                engineerId = rfq.IngenieurRFQId;
                rfqId = rfq.Id;
                entityName = $"RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ})";
            }
            else if (versionRFQ != null)
            {
                engineerId = versionRFQ.IngenieurRFQId;
                rfqId = versionRFQ.RFQId;
                entityName = $"Version RFQ '{versionRFQ.QuoteName}' (CQ: {versionRFQ.CQ})";
            }
            
            if (engineerId.HasValue)
            {
                var message = $"Nouveau commentaire ajouté à {entityName}.";
                
                // Get the current user's name from JWT claims
                var actionUserName = User.FindFirst("name")?.Value ?? 
                                   User.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? 
                                   "Utilisateur inconnu";

                await _notificationService.CreateNotification(message, engineerId.Value, rfqId, actionUserName);

                // Also notify other Validateurs (excluding the commenting Validateur)
                var validateurMessage = $"Nouveau commentaire ajouté à {entityName} par {actionUserName}.";
                await _notificationService.CreateNotificationsForRoleExcluding(validateurMessage, "Validateur", rfqId, actionUserName, validateurId);
            }

            return CreatedAtAction(nameof(Get), new { id = commentaire.Id }, commentaire);
        }





        [HttpGet("{id}")]
        public ActionResult<Commentaire> Get(int id)
        {
            var commentaire = _commentaireService.Get(id);
            if (commentaire == null)
            {
                return NotFound();
            }

            return commentaire;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Validateur")]
        public ActionResult<Commentaire> Update(int id, [FromBody] UpdateCommentaireDto dto)
        {
            if (!TryGetCurrentUserId(out int validateurId))
            {
                return Unauthorized(new { message = "Validateur ID not found in token" });
            }

            var commentaire = _commentaireService.Get(id);
            if (commentaire == null)
            {
                return NotFound();
            }

            commentaire.Contenu = dto.Contenu ?? commentaire.Contenu;

            _commentaireService.Update(commentaire);

            RFQ rfq = null;
            VersionRFQ version = null;

            if (commentaire.RFQId.HasValue)
            {
                rfq = _rfqService.Get(commentaire.RFQId.Value);
            }
            else if (commentaire.VersionRFQId.HasValue)
            {
                version = _versionRFQService.Get(commentaire.VersionRFQId.Value);
            }

            var cibleAction = commentaire.RFQId.HasValue ? "COMMENTAIRE_RFQ" : "COMMENTAIRE_VERSION";
            var referenceCible = commentaire.RFQId.HasValue ? rfq?.CQ.ToString() : version?.CQ.ToString();
            var entityDescription = commentaire.RFQId.HasValue && rfq != null
                ? $"RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ})"
                : commentaire.VersionRFQId.HasValue && version != null
                    ? $"Version RFQ '{version.QuoteName}' (CQ: {version.CQ})"
                    : "élément inconnu";

            _actionHistoryLogger.LogAction(
                "COMMENT_UPDATED",
                cibleAction,
                referenceCible,
                $"Commentaire mis à jour sur {entityDescription}.",
                validateurId);

            // Notify assigned engineer and other validators (excluding the commenting validator)
            int? engineerId = null;
            int rfqId = 0;
            string entityName = "";

            if (rfq != null)
            {
                engineerId = rfq.IngenieurRFQId;
                rfqId = rfq.Id;
                entityName = $"RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ})";
            }
            else if (version != null)
            {
                engineerId = version.IngenieurRFQId;
                rfqId = version.RFQId;
                entityName = $"Version RFQ '{version.QuoteName}' (CQ: {version.CQ})";
            }

            var actionUserName = User.FindFirst("name")?.Value ??
                               User.FindFirst(ClaimTypes.Name)?.Value ??
                               User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ??
                               "Utilisateur inconnu";

            if (engineerId.HasValue)
            {
                var engineerMessage = $"Commentaire mis à jour sur {entityName} par {actionUserName}.";
                _notificationService.CreateNotification(engineerMessage, engineerId.Value, rfqId, actionUserName);
            }

            var validateurMessage = $"Commentaire mis à jour sur {entityName} par {actionUserName}.";
            _notificationService.CreateNotificationsForRoleExcluding(validateurMessage, "Validateur", rfqId, actionUserName, validateurId);

            return Ok(commentaire);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Validateur")]
        public IActionResult Delete(int id)
        {
            if (!TryGetCurrentUserId(out int validateurId))
            {
                return Unauthorized(new { message = "Validateur ID not found in token" });
            }

            var commentaire = _commentaireService.Get(id);
            if (commentaire == null)
            {
                return NotFound();
            }

            RFQ rfq = null;
            VersionRFQ version = null;

            if (commentaire.RFQId.HasValue)
            {
                rfq = _rfqService.Get(commentaire.RFQId.Value);
            }
            else if (commentaire.VersionRFQId.HasValue)
            {
                version = _versionRFQService.Get(commentaire.VersionRFQId.Value);
            }

            _commentaireService.Delete(commentaire);

            var cibleAction = commentaire.RFQId.HasValue ? "COMMENTAIRE_RFQ" : "COMMENTAIRE_VERSION";
            var referenceCible = commentaire.RFQId.HasValue ? rfq?.CQ.ToString() : version?.CQ.ToString();
            var entityDescription = commentaire.RFQId.HasValue && rfq != null
                ? $"RFQ '{rfq.QuoteName}' (CQ: {rfq.CQ})"
                : commentaire.VersionRFQId.HasValue && version != null
                    ? $"Version RFQ '{version.QuoteName}' (CQ: {version.CQ})"
                    : "élément inconnu";

            _actionHistoryLogger.LogAction(
                "COMMENT_DELETED",
                cibleAction,
                referenceCible,
                $"Commentaire supprimé sur {entityDescription}.",
                validateurId);

            return NoContent();
        }

        [HttpGet("byrfq/{rfqId}")]
        public ActionResult<IEnumerable<CommentaireDto>> GetCommentsByRFQ(int rfqId)
        {
            var commentaires = _commentaireService.GetAll()
                .Where(c => c.RFQId == rfqId)
                .Select(c => new CommentaireDto
                {
                    Id = c.Id,
                    Contenu = c.Contenu,
                    DateC = c.DateC,
                    ValidateurId = c.ValidateurId ,
                    NomUser = _userService.Get(c.ValidateurId).NomUser 
                })
                .ToList();

            return Ok(commentaires);
        }


        [HttpGet("byversionrfq/{versionRfqId}")]
        public ActionResult<IEnumerable<CommentaireDto>> GetCommentsByVersionRFQ(int versionRfqId)
        {
            var commentaires = _commentaireService.GetAll()
                 .Where(c => c.VersionRFQId == versionRfqId)
                 .Select(c => new CommentaireDto
                 {
                     Id = c.Id,
                     Contenu = c.Contenu,
                     DateC = c.DateC,
                     ValidateurId = c.ValidateurId ,
                     NomUser = _userService.Get(c.ValidateurId).NomUser

                 })
                 .ToList();

            return Ok(commentaires);
        }
    }

    public class CreateCommentaireDto
    {
        public string Contenu { get; set; }
        public int? RFQId { get; set; }        // Optional
        public int? VersionRFQId { get; set; } // Optional
    }

    public class UpdateCommentaireDto
    {
        public string? Contenu { get; set; }
    }

    public class CommentaireDto
    {
        public int Id { get; set; }
        public string Contenu { get; set; }
        public DateTime DateC { get; set; }
        public int ValidateurId { get; set; }

        public string? NomUser { get; set; }
    }

}
