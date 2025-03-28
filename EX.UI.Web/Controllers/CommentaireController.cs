using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        public CommentaireController(
            IService<Commentaire> commentaireService,
            IService<User> userService,
            IService<RFQ> rfqService,
            IService<VersionRFQ> versionRFQService)
        {
            _commentaireService = commentaireService;
            _userService = userService;
            _rfqService = rfqService;
            _versionRFQService = versionRFQService;
        }

        [HttpPost]
        [Authorize(Roles = "Validateur")]
        public ActionResult<Commentaire> Create([FromBody] CreateCommentaireDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // UserId extraction (reusing logic from GetCurrentUser)
            var userIdClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _))?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int validateurId))
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
            var commentaire = _commentaireService.Get(id);
            if (commentaire == null)
            {
                return NotFound();
            }

            commentaire.Contenu = dto.Contenu ?? commentaire.Contenu;

            _commentaireService.Update(commentaire);
            return Ok(commentaire);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Validateur")]
        public IActionResult Delete(int id)
        {
            var commentaire = _commentaireService.Get(id);
            if (commentaire == null)
            {
                return NotFound();
            }

            _commentaireService.Delete(commentaire);
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
