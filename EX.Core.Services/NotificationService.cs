using EX.Core.Domain;
using EX.Core.Interfaces;

namespace EX.Core.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRealTimeNotificationService _realTimeService;

        public NotificationService(IUnitOfWork unitOfWork, IRealTimeNotificationService realTimeService)
        {
            _unitOfWork = unitOfWork;
            _realTimeService = realTimeService;
        }

        public IEnumerable<Notification> GetUserNotifications(int userId)
        {
            var repository = _unitOfWork.GetRepository<Notification>();
            return repository.GetAll()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public int GetUnreadCount(int userId)
        {
            var repository = _unitOfWork.GetRepository<Notification>();
            return repository.GetAll()
                .Count(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<Notification> CreateNotification(string message, int userId, int rfqId, string actionUserName)
        {
            var notification = new Notification
            {
                Message = message,
                UserId = userId,
                RFQId = rfqId,
                ActionUserName = actionUserName,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            var repository = _unitOfWork.GetRepository<Notification>();
            repository.Add(notification);
            _unitOfWork.Save();

            // Send real-time notification
            await _realTimeService.SendNotificationAsync(userId, notification);

            return notification;
        }

        public bool MarkAsRead(int notificationId, int userId)
        {
            var repository = _unitOfWork.GetRepository<Notification>();
            var notification = repository.GetAll()
                .FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            notification.IsRead = true;
            repository.Update(notification);
            _unitOfWork.Save();

            return true;
        }

        public bool MarkAllAsRead(int userId)
        {
            var repository = _unitOfWork.GetRepository<Notification>();
            var notifications = repository.GetAll()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToList();

            if (!notifications.Any())
            {
                return false;
            }

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                repository.Update(notification);
            }

            _unitOfWork.Save();
            return true;
        }

        public bool DeleteNotification(int notificationId, int userId)
        {
            var repository = _unitOfWork.GetRepository<Notification>();
            var notification = repository.GetAll()
                .FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                return false;

            repository.Delete(notification);
            _unitOfWork.Save();
            return true;
        }

        public async Task CreateNotificationsForRole(string message, string role, int rfqId, string actionUserName)
        {
            var userRepository = _unitOfWork.GetRepository<User>();
            var notificationRepository = _unitOfWork.GetRepository<Notification>();
            
            // Get all users with the specified role
            var users = userRepository.GetAll()
                .Where(u => u.Role.ToString() == role)
                .ToList();

            foreach (var user in users)
            {
                var notification = new Notification
                {
                    Message = message,
                    UserId = user.Id,
                    RFQId = rfqId,
                    ActionUserName = actionUserName,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                notificationRepository.Add(notification);
                
                // Send real-time notification
                await _realTimeService.SendNotificationAsync(user.Id, notification);
            }

            _unitOfWork.Save();
        }

    }
}