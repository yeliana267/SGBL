using Application.Interfaces.Services;
using Application.ViewModels.BookGenre;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/book-genres")]
public sealed class BookGenresController : ControllerBase
{
	private readonly IBookGenreService _service;

	public BookGenresController(IBookGenreService service) => _service = service;

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] BookGenreCreateDto dto, CancellationToken ct)
	{
		var ok = await _service.CreateAsync(dto, ct);
		return ok ? CreatedAtAction(nameof(GetByBook), new { bookId = dto.BookId }, null)
				  : BadRequest("Book or Genre not found.");
	}

	[HttpDelete("{bookId:int}/{genreId:int}")]
	public async Task<IActionResult> Delete(int bookId, int genreId, CancellationToken ct)
	{
		var ok = await _service.DeleteAsync(bookId, genreId, ct);
		return ok ? NoContent() : NotFound();
	}

	[HttpGet("by-book/{bookId:int}")]
	public async Task<IActionResult> GetByBook(int bookId, CancellationToken ct)
	{
		var list = await _service.ListByBookAsync(bookId, ct);
		return Ok(list);
	}

	[HttpGet("by-genre/{genreId:int}")]
	public async Task<IActionResult> GetByGenre(int genreId, CancellationToken ct)
	{
		var list = await _service.ListByGenreAsync(genreId, ct);
		return Ok(list);
	}
}
