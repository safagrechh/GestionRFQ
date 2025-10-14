using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using EX.Core.Domain;
using EX.Core.Services;

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

        public ActionHistoryLogger(IService<HistoriqueAction> historiqueActionService)
        {
            _historiqueActionService = historiqueActionService;
        }

        public void LogAction(string type, string cibleAction, string? referenceCible, string? detailsAction, ClaimsPrincipal user)
        {
            if (user == null)
            {
                return;
            }

            var userId = ExtractUserId(user);
            if (!userId.HasValue)
            {
                return;
            }

            LogAction(type, cibleAction, referenceCible, detailsAction, userId.Value);
        }

        public void LogAction(string type, string cibleAction, string? referenceCible, string? detailsAction, int userId)
        {
            var action = new HistoriqueAction
            {
                Type = type,
                CibleAction = cibleAction,
                ReferenceCible = referenceCible,
                DetailsAction = detailsAction,
                DateAction = DateTime.UtcNow,
                UserId = userId
            };

            _historiqueActionService.Add(action);
        }

        private static int? ExtractUserId(ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user.FindFirst(JwtRegisteredClaimNames.Sub);

            if (claim != null && int.TryParse(claim.Value, out var id))
            {
                return id;
            }

            return null;
        }
    }
}
