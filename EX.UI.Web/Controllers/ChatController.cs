using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EX.UI.Web.Controllers
{
    [Route("chat")]
    public class ChatController : Controller
    {
        private readonly IGenerativeAiClient _ai;
        private readonly IService<RFQ> _rfqService;
        private readonly ILogger<ChatController> _logger;
        private readonly GoogleAIOptions _aiOptions;

        public ChatController(IGenerativeAiClient ai, IService<RFQ> rfqService, ILogger<ChatController> logger, IOptions<GoogleAIOptions> aiOptions)
        {
            _ai = ai;
            _rfqService = rfqService;
            _logger = logger;
            _aiOptions = aiOptions.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public record ChatRequest(string message);
        public record ChatResponse(string reply);

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] ChatRequest req, CancellationToken ct)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.message))
                return BadRequest(new { error = "Message vide" });

            // 1) Réponse directe si demande d’état RFQ clair
            var rfqInfo = TryGetRfqStatus(req.message);
            if (rfqInfo != null)
            {
                return Ok(new ChatResponse(rfqInfo));
            }

            // 2) Sinon déléguer au modèle IA avec un bref contexte
            var messages = new List<(string role, string content)>
            {
                ("user", req.message)
            };

            string reply;
            try
            {
                var temp = _aiOptions.DefaultTemperature;
                var maxTok = _aiOptions.DefaultMaxTokens;
                reply = await _ai.GenerateAsync(messages, temperature: temp, maxTokens: maxTok, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur Gemini lors de la génération de la réponse");
                return StatusCode(502, new { error = "Le service IA est indisponible pour le moment." });
            }

            if (string.IsNullOrWhiteSpace(reply))
            {
                reply = "Je n’ai pas pu générer une réponse. Pouvez-vous reformuler votre question ?";
            }

            return Ok(new ChatResponse(reply));
        }

        private string? TryGetRfqStatus(string message)
        {
            // Exemples: "Où en est la validation de RFQ #123?", "Statut RFQ 123", "RFQ n°123"
            var m = Regex.Match(message, @"RFQ\s*(?:#|n°|numéro|)\s*(\d+)", RegexOptions.IgnoreCase);
            if (!m.Success) return null;
            if (!int.TryParse(m.Groups[1].Value, out var id)) return null;

            var rfq = _rfqService.Get(id);
            if (rfq == null)
                return $"RFQ #{id} introuvable.";

            var statut = rfq.Statut?.ToString() ?? "Non défini";
            var valide = rfq.Valide ? "Validé" : "Non validé";
            var rejete = rfq.Rejete ? "Rejeté" : "Non rejeté";
            var brouillon = rfq.Brouillon ? "Brouillon" : "Publié";

            return $"RFQ #{id}: Statut={statut}, Validation={valide}, Rejet={rejete}, État={brouillon}.";
        }
    }
}