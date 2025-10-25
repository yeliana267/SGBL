
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class UserStatusRepository : GenericRepository<UserStatus>, IUserStatusRepository
    {
        private readonly SGBLContext _context;
        private readonly IServiceLogs _serviceLogs;
        public UserStatusRepository(SGBLContext context, IServiceLogs serviceLogs) : base(context, serviceLogs)
        {
            _serviceLogs = serviceLogs;
            _context = context;
        }
    }
}
