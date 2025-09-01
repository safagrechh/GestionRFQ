using EX.Core.Domain;

namespace EX.Core.Services
{
    public interface INotificationService
    {
        IEnumerable<Notification> GetUserNotifications(int userId);
        int GetUnreadCount(int userId);
        Notification CreateNotification(string message, int userId, int rfqId);
        bool MarkAsRead(int notificationId, int userId);
        bool MarkAllAsRead(int userId);
        bool DeleteNotification(int notificationId, int userId);
    }

    public interface IRealTimeNotificationService
    {
        Task SendNotificationAsync(int userId, Notification notification);
    }
}