using Microsoft.EntityFrameworkCore;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class BookAuthorRepository : GenericRepository<BookAuthor>, IBookAuthorRepository
    {
        private readonly SGBLContext _context;

        public BookAuthorRepository(SGBLContext context, IServiceLogs serviceLogs)
            : base(context, serviceLogs)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookAuthor>> GetByBookIdAsync(int bookId)
        {
            return await _context.BookAuthors
                .Include(ba => ba.Author)
                .Where(ba => ba.IdBook == bookId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookAuthor>> GetByAuthorIdAsync(int authorId)
        {
            return await _context.BookAuthors
                .Include(ba => ba.Book)
                .Where(ba => ba.IdAuthor == authorId)
                .ToListAsync();
        }

        public async Task<bool> RemoveByBookIdAsync(int bookId)
        {
            var bookAuthors = await _context.BookAuthors
                .Where(ba => ba.IdBook == bookId)
                .ToListAsync();

            _context.BookAuthors.RemoveRange(bookAuthors);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task AddRangeAsync(IEnumerable<BookAuthor> bookAuthors)
        {
            await _context.BookAuthors.AddRangeAsync(bookAuthors);
            await _context.SaveChangesAsync();
        }
    }
}