using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;


namespace SGBL.Persistence.Repositories
{
    public class NationalityRepository : GenericRepository<Nationality>, INationalityRepository
    {
        private readonly SGBLContext _context;

        public NationalityRepository(SGBLContext context) : base(context)
        {
            _context = context;
        }
    }
}
