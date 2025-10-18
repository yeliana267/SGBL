
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class UserStatusRepository : GenericRepository<UserStatus>, IUserStatusRepository
    {
        private readonly SGBLContext _context;
        public UserStatusRepository(SGBLContext context) : base(context)
        {
            _context = context;
        }
    }
}
