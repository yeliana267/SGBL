using System.Collections.Generic;
using SGBL.Application.ViewModels;

namespace SGBL.Web.ViewModels
{
    public class UserNotificationWidgetViewModel
    {
        public IReadOnlyList<NotificationViewModel> Notifications { get; init; } = new List<NotificationViewModel>();
        public int UnreadCount { get; init; }
    }
}
