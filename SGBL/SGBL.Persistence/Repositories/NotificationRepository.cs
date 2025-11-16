using System;
using System.Collections.Generic;
using System.Linq;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;
using Microsoft.EntityFrameworkCore;


namespace SGBL.Persistence.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly SGBLContext _context;
        private readonly IServiceLogs _serviceLogs;
        public NotificationRepository(SGBLContext context, IServiceLogs serviceLogs) : base(context, serviceLogs)
        {
            _context = context;
            _serviceLogs = serviceLogs;
        }
        public async Task<List<Notification>> GetByUserAsync(int userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.IdUser == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadByUserAsync(int userId, int unreadStatus)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.IdUser == userId && n.Status == unreadStatus)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountUnreadByUserAsync(int userId, int unreadStatus)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(n => n.IdUser == userId && n.Status == unreadStatus)
                .CountAsync();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int readStatus)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                return false;
            }

            notification.Status = readStatus;
            notification.ReadDate = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> MarkAllAsReadAsync(int userId, int readStatus, int unreadStatus)
        {
            var notifications = await _context.Notifications
                .Where(n => n.IdUser == userId && n.Status == unreadStatus)
                .ToListAsync();

            if (!notifications.Any())
            {
                return 0;
            }

            foreach (var notification in notifications)
            {
                notification.Status = readStatus;
                notification.ReadDate ??= DateTime.UtcNow;
                notification.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return notifications.Count;
        }
    }
}
