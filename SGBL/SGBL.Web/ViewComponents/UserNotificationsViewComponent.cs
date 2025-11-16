using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;
using SGBL.Web.ViewModels;

namespace SGBL.Web.ViewComponents
{
    public class UserNotificationsViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public UserNotificationsViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int take = 5)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
            {
                return View(new UserNotificationWidgetViewModel());
            }

            var userIdValue = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdValue, out var userId))
            {
                return View(new UserNotificationWidgetViewModel());
            }

            var notifications = await _notificationService.GetRecentByUserAsync(userId, take, onlyUnread: false);

            var items = notifications
                .Select(notification => new NotificationViewModel
                {
                    Id = notification.Id,
                    IdUser = notification.IdUser,
                    Type = notification.Type,
                    Title = notification.Title,
                    Message = notification.Message,
                    Status = notification.Status,
                    IdBook = notification.IdBook,
                    IdLoan = notification.IdLoan,
                    ReadDate = notification.ReadDate,
                    CreatedAt = notification.CreatedAt,
                    UpdatedAt = notification.UpdatedAt
                })
                .ToList();

            var widgetModel = new UserNotificationWidgetViewModel
            {
                Notifications = items,
                UnreadCount = items.Count(n => !n.IsRead)
            };

            return View(widgetModel);
        }
    }
}
