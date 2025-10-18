using Application.ViewModels.BookGenre;

namespace Application.Interfaces.Services;

public interface IBookGenreService
{
    Task<bool> CreateAsync(BookGenreCreateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int bookId, int genreId, CancellationToken ct = default);
    Task<IReadOnlyList<BookGenreListItemDto>> ListByBookAsync(int bookId, CancellationToken ct = default);
    Task<IReadOnlyList<BookGenreListItemDto>> ListByGenreAsync(int genreId, CancellationToken ct = default);
}
