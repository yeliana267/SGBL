using Xunit;
using Moq;
using AutoMapper;
using SGBL.Application.Dtos.Book;
using SGBL.Application.Services;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using SGBL.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGBL.Application.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IServiceLogs> _mockServiceLogs;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockServiceLogs = new Mock<IServiceLogs>();
            _bookService = new BookService(_mockBookRepository.Object, _mockMapper.Object, _mockServiceLogs.Object);
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
    }
}