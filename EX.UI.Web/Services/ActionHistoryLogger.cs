using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EX.UI.Web.Services
{
    public interface IActionHistoryLogger
    {
        void LogAction(string type, string cibleAction, string? referenceCible, string? detailsAction, ClaimsPrincipal user);
        void LogAction(string type, string cibleAction, string? referenceCible, string? detailsAction, int userId);
    }

    public class ActionHistoryLogger : IActionHistoryLogger
    {
        private readonly IService<HistoriqueAction> _historiqueActionService;
        private readonly IService<User> _userService;
        private readonly ILogger<ActionHistoryLogger> _logger;
        private readonly int? _defaultSystemUserId;

        public ActionHistoryLogger(
            IService<HistoriqueAction> historiqueActionService,
            IService<User> userService,
            ILogger<ActionHistoryLogger> logger,
            IConfiguration configuration)
        {
            _historiqueActionService = historiqueActionService;
            _userService = userService;
            _logger = logger;
            // Optional fallback if user ID cannot be resolved from claims
            _defaultSystemUserId = configuration.GetValue<int?>("Audit:DefaultSystemUserId");
        }

        public void LogAction(string type, string cibleAction, string? referenceCible, string? detailsAction, ClaimsPrincipal user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to log action '{Type}' with null ClaimsPrincipal.", type);
                return;
            }

            var userId = ExtractUserId(user);
            if (!userId.HasValue)
            {
                if (_defaultSystemUserId.HasValue)
                {
                    _logger.LogWarning(
                        "Could not resolve user ID from claims; falling back to default system user id {SystemUserId} for type '{Type}'.",
                        _defaultSystemUserId, type);
                    userId = _defaultSystemUserId.Value;
                }
                else
                {
                    _logger.LogWarning("Could not resolve user ID from claims and no default configured; skipping log for type '{Type}'.", type);
                    return;
                }
            }

            LogAction(type, cibleAction, referenceCible, detailsAction, userId.Value);
        }

        public void LogAction(string type, string cibleAction, string? referenceCible, string? detailsAction, int userId)
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(cibleAction))
            {
                _logger.LogWarning("Invalid audit log payload: Type or CibleAction is missing.");
                return;
            }

            var action = new HistoriqueAction
            {
                Type = type.Trim(),
                CibleAction = cibleAction.Trim(),
                ReferenceCible = string.IsNullOrWhiteSpace(referenceCible) ? "N/A" : referenceCible.Trim(),
                DetailsAction = string.IsNullOrWhiteSpace(detailsAction) ? "N/A" : detailsAction.Trim(),
                DateAction = DateTime.UtcNow,
                UserId = userId
            };

            try
            {
                _historiqueActionService.Add(action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist HistoriqueAction: {Type} on {CibleAction} ref {ReferenceCible}", action.Type, action.CibleAction, action.ReferenceCible);
            }
        }

        private int? ExtractUserId(ClaimsPrincipal user)
        {
            // Try direct numeric claims first
            var idClaim = user.FindFirst(ClaimTypes.NameIdentifier) ?? user.FindFirst(JwtRegisteredClaimNames.Sub);
            if (idClaim != null && int.TryParse(idClaim.Value, out var id))
            {
                return id;
            }

            // Fallback to email claims
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? user.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            if (!string.IsNullOrWhiteSpace(email))
            {
                try
                {
                    var matchedUser = _userService.GetAll().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
                    if (matchedUser != null)
                    {
                        return matchedUser.Id;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while resolving user by email '{Email}'", email);
                }
            }

            // Sometimes Name contains an email or username; try best-effort
            var name = user.FindFirst("name")?.Value ?? user.FindFirst(ClaimTypes.Name)?.Value ?? user.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                try
                {
                    var matchedUser = _userService.GetAll().FirstOrDefault(u => 
                        u.Email.Equals(name, StringComparison.OrdinalIgnoreCase) ||
                        (u.NomUser != null && u.NomUser.Equals(name, StringComparison.OrdinalIgnoreCase))
                    );
                    if (matchedUser != null)
                    {
                        return matchedUser.Id;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while resolving user by name '{Name}'", name);
                }
            }

            return null;
        }
    }
}
