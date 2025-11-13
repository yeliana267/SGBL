using Microsoft.EntityFrameworkCore;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Persistence.Base;
using SGBL.Persistence.Context;

namespace SGBL.Persistence.Repositories
{
    public class BookGenreRepository : GenericRepository<BookGenre>, IBookGenreRepository
    {
        private readonly SGBLContext _context;

        public BookGenreRepository(SGBLContext context, IServiceLogs serviceLogs)
            : base(context, serviceLogs)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookGenre>> GetByBookIdAsync(int bookId)
        {
            return await _context.BookGenres
                .Include(ba => ba.Genre)
                .Where(ba => ba.IdBook == bookId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookGenre>> GetByGenreIdAsync(int authorId)
        {
            return await _context.BookGenres
                .Include(ba => ba.Book)
                .Where(ba => ba.IdGenre == authorId)
                .ToListAsync();
        }

        public async Task<bool> RemoveByBookIdAsync(int bookId)
        {
            var bookGenres = await _context.BookGenres
                .Where(ba => ba.IdBook == bookId)
                .ToListAsync();

            _context.BookGenres.RemoveRange(bookGenres);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task AddRangeAsync(IEnumerable<BookGenre> bookGenres)
        {
            await _context.BookGenres.AddRangeAsync(bookGenres);
            await _context.SaveChangesAsync();
        }
    }
}