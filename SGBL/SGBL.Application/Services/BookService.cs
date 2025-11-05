using AutoMapper;
using SGBL.Application.Dtos.Author;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;


namespace SGBL.Application.Services
{
    public class BookService : GenericService<Book, BookDto>, IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        private readonly IBookAuthorRepository _bookAuthorRepository;
        private readonly IBookGenreRepository _bookGenreRepository;

        public BookService(IBookRepository bookRepository, IMapper mapper, IServiceLogs serviceLogs, IBookAuthorRepository bookAuthorRepository, IBookGenreRepository bookgenreRepository)
            : base(bookRepository, mapper, serviceLogs)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
            _bookAuthorRepository = bookAuthorRepository;
            _bookGenreRepository = bookgenreRepository;
        }
        //autores

        public async Task<List<AuthorDto>> GetBookAuthors(int bookId)
        {
            var bookAuthors = await _bookAuthorRepository.GetByBookIdAsync(bookId);
            return _mapper.Map<List<AuthorDto>>(bookAuthors.Select(ba => ba.Author));
        }
        public async Task UpdateBookAuthors(int bookId, List<int> authorIds)
        {
            // Eliminar relaciones existentes
            await _bookAuthorRepository.RemoveByBookIdAsync(bookId);

            // Agregar nuevas relaciones
            if (authorIds.Any())
            {
                await AddAuthorsToBook(bookId, authorIds);
            }
        }
        public async Task AddAuthorsToBook(int bookId, List<int> authorIds)
        {
            var bookAuthors = authorIds.Select(authorId => new BookAuthor
            {
                IdBook = bookId,
                IdAuthor = authorId
            });

            await _bookAuthorRepository.AddRangeAsync(bookAuthors);
        }
        //generos
        public async Task AddGenresToBook(int bookId, List<int> genreIds)
        {
            var bookGenres = genreIds.Select(genreId => new BookGenre
            {
                IdBook = bookId,
                IdGenre = genreId
            });

            await _bookGenreRepository.AddRangeAsync(bookGenres);
        }
        public async Task UpdateBookGenres(int bookId, List<int> genreIds)
        {
            // Eliminar relaciones existentes
            await _bookGenreRepository.RemoveByBookIdAsync(bookId);

            // Agregar nuevas relaciones
            if (genreIds.Any())
            {
                await AddGenresToBook(bookId, genreIds);
            }
        }

        public async Task<List<GenreDto>> GetBookGenres(int bookId)
        {
            var bookgenres = await _bookGenreRepository.GetByBookIdAsync(bookId);
            return _mapper.Map<List<GenreDto>>(bookgenres.Select(bg => bg.Genre));
        }

        public async Task<BookDto> AddAsync(BookDto dto)
        {
            //  VALIDAR ISBN DUPLICADO - CORREGIDO: Verificar null
            var existingBooks = await _bookRepository.GetAllAsync();
            if (existingBooks != null && existingBooks.Any(b => b.Isbn == dto.Isbn))
            {
                throw new System.InvalidOperationException("ISBN ya registrado");
            }

            // VALIDAR STOCK NEGATIVO
            if (dto.AvailableCopies < 0 || dto.TotalCopies < 0)
            {
                throw new System.ArgumentException("El stock no puede ser negativo");
            }

            //  VALIDAR TÍTULO VACÍO
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new System.ArgumentException("El título es requerido");
            }

            //  VALIDAR AÑO DE PUBLICACIÓN
            var currentYear = System.DateTime.Now.Year;
            if (dto.PublicationYear < 1000 || dto.PublicationYear > currentYear + 1)
            {
                throw new System.ArgumentException($"El año de publicación debe estar entre 1000 y {currentYear + 1}");
            }

            var entity = _mapper.Map<Book>(dto);
            await _bookRepository.AddAsync(entity);
            return _mapper.Map<BookDto>(entity);
        }
    }
}