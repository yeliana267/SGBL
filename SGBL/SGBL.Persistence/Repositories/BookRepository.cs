using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly SGBLContext _context;
        private readonly IServiceLogs _serviceLogs;
        public BookRepository(SGBLContext context, IServiceLogs serviceLogs): base(context, serviceLogs) 
        {
            _context = context;
            _serviceLogs = serviceLogs;
        }
    }
}
