using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly SGBLContext _context;
        public BookRepository(SGBLContext context): base(context) 
        {
            _context = context;
        }
    }
}
