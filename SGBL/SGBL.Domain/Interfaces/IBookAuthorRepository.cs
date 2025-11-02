using SGBL.Domain.Entities;


namespace SGBL.Domain.Interfaces
{
    public interface IBookAuthorRepository : IGenericRepository<BookAuthor>

    {

        Task<IEnumerable<BookAuthor>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<BookAuthor>> GetByAuthorIdAsync(int authorId);
        Task<bool> RemoveByBookIdAsync(int bookId);
        Task AddRangeAsync(IEnumerable<BookAuthor> bookAuthors);
    }
}
