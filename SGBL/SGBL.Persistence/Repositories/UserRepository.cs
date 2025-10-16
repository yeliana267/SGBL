

using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly SGBLContext _context;

        public UserRepository(SGBLContext context) : base(context)
        {
            _context = context;
        }
    }
}
