using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        private readonly SGBLContext _context;

        public RoleRepository(SGBLContext context) : base(context)
        {
            _context = context;

        }
    }
}
