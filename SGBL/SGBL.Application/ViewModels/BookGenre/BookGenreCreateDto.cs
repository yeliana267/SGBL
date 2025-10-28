using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.BookGenre;

public sealed class BookGenreCreateDto
{
    [Required] public int BookId { get; set; }
    [Required] public int GenreId { get; set; }
}
