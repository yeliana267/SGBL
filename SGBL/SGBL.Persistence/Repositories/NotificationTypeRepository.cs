
using SGBL.Application.Interfaces;
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
        private readonly IServiceLogs _serviceLogs;
        public NotificationTypeRepository(SGBLContext context, IServiceLogs serviceLogs) : base(context, serviceLogs)
        {
            _serviceLogs = serviceLogs;
            _context = context;
        }
        
    }
}
