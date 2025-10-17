
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;
using System;

namespace SGBL.Persistence.Repositories
{
    public class NotificationStatusRepository : GenericRepository<NotificationStatus>, INotificationStatusRepository
    {
        private readonly SGBLContext _context;
        public NotificationStatusRepository(SGBLContext context) : base(context)
        {
            _context = context;
        }
        
    }
}
