using System.Collections.Generic;

namespace SGBL.Application.ViewModels
{
    public class NotificationIndexViewModel
    {
        public int? UserId { get; set; }
        public List<NotificationViewModel> Notifications { get; set; } = new();
        public List<NotificationViewModel> UnreadNotifications { get; set; } = new();
        public int UnreadCount => UnreadNotifications?.Count ?? 0;
    }
}