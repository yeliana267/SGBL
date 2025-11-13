using SGBL.Domain.Entities;


namespace SGBL.Domain.Interfaces
{
    public interface IBookGenreRepository : IGenericRepository<BookGenre>

    {

        Task<IEnumerable<BookGenre>> GetByBookIdAsync(int bookId);
        Task<IEnumerable<BookGenre>> GetByGenreIdAsync(int genreId);
        Task<bool> RemoveByBookIdAsync(int bookId);
        Task AddRangeAsync(IEnumerable<BookGenre> bookGenres);
    }
}
