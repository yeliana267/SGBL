

using SGBL.Application.Dtos.Author;
using SGBL.Application.Dtos.Book;

namespace SGBL.Application.Interfaces
{
    public interface IBookService : IGenericService<BookDto>
    {
        Task AddAuthorsToBook(int bookId, List<int> authorIds);
        Task UpdateBookAuthors(int bookId, List<int> authorIds);
        Task<List<AuthorDto>> GetBookAuthors(int bookId);
        Task AddGenresToBook(int bookId, List<int> genreIds);
        Task UpdateBookGenres(int bookId, List<int> genreIds);
        Task<List<GenreDto>> GetBookGenres(int bookId);
    }
}
