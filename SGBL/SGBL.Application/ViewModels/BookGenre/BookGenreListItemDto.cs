namespace Application.ViewModels.BookGenre;

public sealed class BookGenreListItemDto
{
    public int BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;

    public int GenreId { get; set; }
    public string GenreName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
