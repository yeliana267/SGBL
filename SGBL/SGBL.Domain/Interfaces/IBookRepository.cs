
using System.Collections.Generic;
using SGBL.Domain.Entities;

namespace SGBL.Domain.Interfaces
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<(List<Book> Books, int TotalCount)> SearchBooksPagedAsync(
    string? title,
    int? genreId,
    int? authorId,
    int pageNumber,
    int pageSize);
        Task<Book?> AdjustAvailableCopiesAsync(int bookId, int delta);
    }
}