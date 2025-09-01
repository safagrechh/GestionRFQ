using EX.Core.Domain;
using EX.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace EX.UI.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("test")]
        public IActionResult TestAuth()
        {
            // Get all NameIdentifier claims and find the numeric one (user ID)
            var nameIdentifierClaims = User.FindAll(ClaimTypes.NameIdentifier);
            var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => int.TryParse(c.Value, out _))?.Value;
            var emailClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            
            return Ok(new { 
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserId = userIdClaim,
                Email = emailClaim,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        [HttpGet]
        public IActionResult GetNotifications()
        {
            try
            {
                // Get user ID from JWT token
                var nameIdentifierClaims = User.FindAll(ClaimTypes.NameIdentifier);
                var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => int.TryParse(c.Value, out _))?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user token");
                }

                var notifications = _notificationService.GetUserNotifications(userId);

                var result = notifications.Select(n => new
                {
                    n.Id,
                    n.Message,
                    n.CreatedAt,
                    n.IsRead,
                    n.RFQId,
                    n.UserId,
                    RFQTitle = n.RFQ?.QuoteName ?? "Unknown"
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving notifications: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Message) || request.UserId <= 0)
                {
                    return BadRequest("Invalid notification data");
                }

                var notification = _notificationService.CreateNotification(
                    request.Message, request.UserId, request.RFQId);

                return Ok(new { message = "Notification created successfully", notificationId = notification.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating notification: {ex.Message}");
            }
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                // Get user ID from JWT token
                var nameIdentifierClaims = User.FindAll(ClaimTypes.NameIdentifier);
                var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => int.TryParse(c.Value, out _))?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user token");
                }

                var success = _notificationService.MarkAsRead(id, userId);
                
                if (!success)
                {
                    return NotFound("Notification not found or access denied");
                }

                return Ok(new { message = "Notification marked as read" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error marking notification as read: {ex.Message}");
            }
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                // Get user ID from JWT token
                var nameIdentifierClaims = User.FindAll(ClaimTypes.NameIdentifier);
                var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => int.TryParse(c.Value, out _))?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user token");
                }

                var count = _notificationService.GetUnreadCount(userId);

                return Ok(new { unreadCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error getting unread count: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                // Get user ID from JWT token
                var nameIdentifierClaims = User.FindAll(ClaimTypes.NameIdentifier);
                var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => int.TryParse(c.Value, out _))?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized("Invalid user token");
                }

                var success = _notificationService.DeleteNotification(id, userId);
                
                if (!success)
                {
                    return NotFound("Notification not found or access denied");
                }

                return Ok(new { message = "Notification deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting notification: {ex.Message}");
            }
        }
    }

    public class CreateNotificationRequest
    {
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int RFQId { get; set; }
    }
}
