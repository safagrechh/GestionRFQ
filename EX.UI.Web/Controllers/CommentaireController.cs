using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EX.UI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Important for swagger
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
        public ActionResult<Commentaire> Create([FromBody] CreateCommentaireDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validation: Comment should be either on RFQ or VersionRFQ, but not both
            if (dto.RFQId.HasValue && dto.VersionRFQId.HasValue)
            {
                return BadRequest("A comment can only be associated with either an RFQ or a VersionRFQ, not both.");
            }

            if (!dto.RFQId.HasValue && !dto.VersionRFQId.HasValue)
            {
                return BadRequest("A comment must be associated with either an RFQ or a VersionRFQ.");
            }

            // Validate Validateur
            var validateur = _userService.Get(dto.ValidateurId);
            if (validateur == null)
            {
                return NotFound("Validateur not found.");
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

            // Create Commentaire
            var commentaire = new Commentaire
            {
                Contenu = dto.Contenu,
                DateC = DateTime.UtcNow,
                ValidateurId = dto.ValidateurId,
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
    }

    public class CreateCommentaireDto
    {
        public string Contenu { get; set; }
        public int ValidateurId { get; set; } // Required
        public int? RFQId { get; set; }        // Optional
        public int? VersionRFQId { get; set; } // Optional
    }

    public class UpdateCommentaireDto
    {
        public string? Contenu { get; set; }
    }
}
