using Microsoft.EntityFrameworkCore;
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
        public async Task<(List<Book> Books, int TotalCount)> SearchBooksPagedAsync(
    string? title,
    int? genreId,
    int? authorId,
    int pageNumber,
    int pageSize)
        {
            var query = _context.Books
      .Include(b => b.BookGenre)
          .ThenInclude(bg => bg.Genre)
      .Include(b => b.BookAuthors)
          .ThenInclude(ba => ba.Author)
      .AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(b => b.Title.Contains(title));

            if (genreId.HasValue)
                query = query.Where(b => b.BookGenre.Any(bg => bg.Genre.Id == genreId.Value));

            if (authorId.HasValue)
                query = query.Where(b => b.BookAuthors.Any(ba => ba.Author.Id == authorId.Value));
            if (string.IsNullOrEmpty(title) && !genreId.HasValue && !authorId.HasValue)
            {
                query = query.OrderByDescending(b => b.CreatedAt);
            }

            int totalCount = await query.CountAsync();

            var books = await query
                .OrderBy(b => b.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            return (books, totalCount);
        }

    }
}
