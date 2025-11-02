

using SGBL.Application.Dtos.Author;
using SGBL.Application.Dtos.Book;

namespace SGBL.Application.Interfaces
{
    public interface IBookService : IGenericService<BookDto>
    {
        Task AddAuthorsToBook(int bookId, List<int> authorIds);
        Task UpdateBookAuthors(int bookId, List<int> authorIds);
        Task<List<AuthorDto>> GetBookAuthors(int bookId);
    }
}
