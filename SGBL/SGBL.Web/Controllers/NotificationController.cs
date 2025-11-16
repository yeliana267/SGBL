using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SGBL.Application.Interfaces;
using SGBL.Application.ViewModels;

namespace SGBL.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int? userId)
        {
            var notifications = userId.HasValue
                ? await _notificationService.GetByUserAsync(userId.Value)
                : await _notificationService.GetAll();

            var unread = userId.HasValue
                ? await _notificationService.GetUnreadByUserAsync(userId.Value)
                : notifications.Where(n => !n.ReadDate.HasValue).ToList();

            var viewModel = new NotificationIndexViewModel
            {
                UserId = userId,
                Notifications = _mapper.Map<List<NotificationViewModel>>(notifications),
                UnreadNotifications = _mapper.Map<List<NotificationViewModel>>(unread)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {

            var notification = await _notificationService.GetById(id);
            if (notification is null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<NotificationViewModel>(notification);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id, int? userId)
        {
            var updated = await _notificationService.MarkAsReadAsync(id);

            TempData[updated ? "ok" : "error"] = updated
                ? "Notificación marcada como leída."
                : "No fue posible actualizar la notificación.";

            return RedirectToAction(nameof(Index), new { userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead(int userId)
        {
            if (userId <= 0)
            {
                TempData["error"] = "Debe seleccionar un usuario para marcar todas las notificaciones como leídas.";
                return RedirectToAction(nameof(Index));
            }

            var updatedCount = await _notificationService.MarkAllAsReadAsync(userId);

            TempData[updatedCount > 0 ? "ok" : "info"] = updatedCount > 0
                ? $"{updatedCount} notificaciones marcadas como leídas."
                : "No se encontraron notificaciones pendientes.";

            return RedirectToAction(nameof(Index), new { userId });
        }


    }
}
