using AutoMapper;
using Moq;
using SGBL.Application.Dtos.Author;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;


namespace SGBL.Application.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IServiceLogs> _mockServiceLogs;
        private readonly Mock<IBookAuthorRepository> _mockBookAuthorRepository;
        private readonly Mock<IBookGenreRepository> _mockBookGenreRepository;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockServiceLogs = new Mock<IServiceLogs>();
            _mockBookAuthorRepository = new Mock<IBookAuthorRepository>(); // ⭐ NUEVO MOCK
            _mockBookGenreRepository = new Mock<IBookGenreRepository>();

            _bookService = new BookService(
                _mockBookRepository.Object,
                _mockMapper.Object,
                _mockServiceLogs.Object,
                _mockBookAuthorRepository.Object,
                _mockBookGenreRepository.Object
            );
        }

        [Fact]
        public async Task AddBook_Should_Throw_When_TitleEmpty()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Title = "",
                Isbn = 9788497941345,
                PublicationYear = 2024,
                Pages = 300,
                TotalCopies = 5,
                AvailableCopies = 5,
                Ubication = "Test Shelf",
                StatusId = 1
            };

            // Act & Assert
            await Xunit.Assert.ThrowsAsync<System.ArgumentException>(() => _bookService.AddAsync(bookDto));
        }

        [Fact]
        public async Task AddBook_Should_Return_With_Valid_Id()
        {
            // Arrange
            var bookDto = new BookDto
            {
                Title = "Test Book",
                Isbn = 9788497941345,
                AvailableCopies = 5,
                TotalCopies = 5,
                PublicationYear = 2024,
                Pages = 300,
                Ubication = "Test Shelf",
                StatusId = 1
            };

            var bookEntity = new Book
            {
                Id = 1,
                Title = "Test Book",
                Isbn = 9788497941345,
                AvailableCopies = 5,
                TotalCopies = 5,
                PublicationYear = 2024,
                Pages = 300,
                Ubication = "Test Shelf",
                Status = 1
            };

            var expectedBookDto = new BookDto
            {
                Id = 1,
                Title = "Test Book"
            };

            _mockMapper.Setup(m => m.Map<Book>(It.IsAny<BookDto>()))
                      .Returns(bookEntity);
            _mockMapper.Setup(m => m.Map<BookDto>(It.IsAny<Book>()))
                      .Returns(expectedBookDto);

            // 👇 CORRECCIÓN: Devolver lista vacía en lugar de null
            _mockBookRepository.Setup(r => r.GetAllAsync())
                             .ReturnsAsync(new List<Book>()); // Lista vacía, no null

            _mockBookRepository.Setup(r => r.AddAsync(It.IsAny<Book>()))
                             .ReturnsAsync(bookEntity);

            // Act
            var result = await _bookService.AddAsync(bookDto);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(1, result.Id);
            Xunit.Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public async Task AddAuthorsToBook_Should_Call_Repository()
        {
            // Arrange
            var bookId = 1;
            var authorIds = new List<int> { 1, 2, 3 };

            _mockBookAuthorRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<BookAuthor>>()))
                                   .Returns(Task.CompletedTask);

            // Act
            await _bookService.AddAuthorsToBook(bookId, authorIds);

            // Assert
            _mockBookAuthorRepository.Verify(
                r => r.AddRangeAsync(It.IsAny<IEnumerable<BookAuthor>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateBookAuthors_Should_Remove_And_Add()
        {
            // Arrange
            var bookId = 1;
            var authorIds = new List<int> { 1, 2, 3 };

            _mockBookAuthorRepository.Setup(r => r.RemoveByBookIdAsync(bookId))
                                   .ReturnsAsync(true);
            _mockBookAuthorRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<BookAuthor>>()))
                                   .Returns(Task.CompletedTask);

            // Act
            await _bookService.UpdateBookAuthors(bookId, authorIds);

            // Assert
            _mockBookAuthorRepository.Verify(r => r.RemoveByBookIdAsync(bookId), Times.Once);
            _mockBookAuthorRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<BookAuthor>>()), Times.Once);
        }

        [Fact]
        public async Task GetBookAuthors_Should_Return_Authors()
        {
            // Arrange
            var bookId = 1;
            var bookAuthors = new List<BookAuthor>
            {
                new BookAuthor { IdBook = 1, IdAuthor = 1, Author = new Author { Id = 1, Name = "Author 1" } },
                new BookAuthor { IdBook = 1, IdAuthor = 2, Author = new Author { Id = 2, Name = "Author 2" } }
            };

            _mockBookAuthorRepository.Setup(r => r.GetByBookIdAsync(bookId))
                                   .ReturnsAsync(bookAuthors);

            var authorDtos = new List<AuthorDto>
            {
                new AuthorDto { Id = 1, Name = "Author 1" },
                new AuthorDto { Id = 2, Name = "Author 2" }
            };

            _mockMapper.Setup(m => m.Map<List<AuthorDto>>(It.IsAny<IEnumerable<Author>>()))
                      .Returns(authorDtos);

            // Act
            var result = await _bookService.GetBookAuthors(bookId);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal(2, result.Count);
            Xunit.Assert.Contains(result, a => a.Name == "Author 1");
            Xunit.Assert.Contains(result, a => a.Name == "Author 2");
        }
    }
}