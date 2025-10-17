
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class BookStatusRepository : GenericRepository<BookStatus>, IBookStatusRepository
    {
        private readonly SGBLContext _context;
        public BookStatusRepository(SGBLContext context) : base(context)
        {
            _context = context;
        }
    }
}
