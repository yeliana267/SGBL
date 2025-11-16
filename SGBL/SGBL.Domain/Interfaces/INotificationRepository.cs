using SGBL.Domain.Entities;

namespace SGBL.Domain.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetByUserAsync(int userId);
        Task<List<Notification>> GetUnreadByUserAsync(int userId, int unreadStatus);
        Task<int> CountUnreadByUserAsync(int userId, int unreadStatus);
        Task<bool> MarkAsReadAsync(int notificationId, int readStatus);
        Task<int> MarkAllAsReadAsync(int userId, int readStatus, int unreadStatus);
    }
}
