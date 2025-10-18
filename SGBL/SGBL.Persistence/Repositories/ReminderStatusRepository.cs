
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class ReminderStatusRepository : GenericRepository<ReminderStatus>, IReminderStatusRepository
    {
        private readonly SGBLContext _context;
        public ReminderStatusRepository(SGBLContext context) : base(context)
        {
            _context = context;
        }
    }
}
