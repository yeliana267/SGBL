using Application.Interfaces.Services;
using Application.ViewModels.BookGenre;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class BookGenreService : IBookGenreService
{
    private readonly AppDbContext _db;

    public BookGenreService(AppDbContext db) => _db = db;

    public async Task<bool> CreateAsync(BookGenreCreateDto dto, CancellationToken ct = default)
    {
        var exists = await _db.BookGenres.AnyAsync(
            x => x.BookId == dto.BookId && x.GenreId == dto.GenreId, ct);
        if (exists) return true; // idempotent

        var bookExists = await _db.Books.AnyAsync(b => b.Id == dto.BookId, ct);
        var genreExists = await _db.Genres.AnyAsync(g => g.Id == dto.GenreId, ct);
        if (!bookExists || !genreExists) return false;

        _db.BookGenres.Add(new BookGenre { BookId = dto.BookId, GenreId = dto.GenreId });
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int bookId, int genreId, CancellationToken ct = default)
    {
        var entity = await _db.BookGenres.FirstOrDefaultAsync(
            x => x.BookId == bookId && x.GenreId == genreId, ct);
        if (entity is null) return false;

        _db.BookGenres.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<BookGenreListItemDto>> ListByBookAsync(int bookId, CancellationToken ct = default)
    {
        return await _db.BookGenres
            .AsNoTracking()
            .Where(x => x.BookId == bookId)
            .Select(x => new BookGenreListItemDto
            {
                BookId = x.BookId,
                BookTitle = x.Book!.Title,
                GenreId = x.GenreId,
                GenreName = x.Genro!.Name // If your property is 'Nombre', adjust accordingly
            })
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<BookGenreListItemDto>> ListByGenreAsync(int genreId, CancellationToken ct = default)
    {
        return await _db.BookGenres
            .AsNoTracking()
            .Where(x => x.GenreId == genreId)
            .Select(x => new BookGenreListItemDto
            {
                BookId = x.BookId,
                BookTitle = x.Book!.Title,
                GenreId = x.GenreId,
                GenreName = x.Genre!.Name,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(ct);
    }
}
