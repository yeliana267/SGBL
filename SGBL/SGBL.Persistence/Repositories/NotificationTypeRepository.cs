
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;
using System;

namespace SGBL.Persistence.Repositories
{
    public class NotificationTypeRepository : GenericRepository<NotificationType>, INotificationTypeRepository
    {
        private readonly SGBLContext _context;
        public NotificationTypeRepository(SGBLContext context) : base(context)
        {
            _context = context;
        }
        
    }
}
