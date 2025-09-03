using EX.Core.Domain;

namespace EX.Core.Services
{
    public interface INotificationService
    {
        IEnumerable<Notification> GetUserNotifications(int userId);
        int GetUnreadCount(int userId);
        Task<Notification> CreateNotification(string message, int userId, int rfqId, string actionUserName);
        Task CreateNotificationsForRole(string message, string role, int rfqId, string actionUserName);
        bool MarkAsRead(int notificationId, int userId);
        bool MarkAllAsRead(int userId);
        bool DeleteNotification(int notificationId, int userId);
    }

    public interface IRealTimeNotificationService
    {
        Task SendNotificationAsync(int userId, Notification notification);
    }
}