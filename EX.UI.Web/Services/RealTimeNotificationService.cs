using EX.Core.Domain;
using EX.Core.Services;
using EX.UI.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EX.UI.Web.Services
{
    public class RealTimeNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<RealTimeNotificationService> _logger;

        public RealTimeNotificationService(IHubContext<NotificationHub> hubContext, ILogger<RealTimeNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendNotificationAsync(int userId, Notification notification)
        {
            try
            {
                var groupName = $"User_{userId}";
                var isUserConnected = NotificationHub.IsUserConnected(userId.ToString());
                
                _logger.LogInformation("Attempting to send real-time notification to user {UserId}, Group: {GroupName}, IsConnected: {IsConnected}", 
                    userId, groupName, isUserConnected);

                var notificationData = new
                {
                    notification.Id,
                    notification.Message,
                    notification.CreatedAt,
                    notification.IsRead,
                    notification.RFQId,
                    notification.UserId,
                    notification.ActionUserName
                };

                await _hubContext.Clients.Group(groupName)
                    .SendAsync("ReceiveNotification", notificationData);
                    
                _logger.LogInformation("Successfully sent real-time notification {NotificationId} to user {UserId}", 
                    notification.Id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send real-time notification {NotificationId} to user {UserId}: {ErrorMessage}", 
                    notification.Id, userId, ex.Message);
            }
        }
    }
}