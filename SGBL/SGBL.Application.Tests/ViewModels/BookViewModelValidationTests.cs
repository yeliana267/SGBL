using Xunit;
using System.ComponentModel.DataAnnotations;
using SGBL.Application.ViewModels;

namespace SGBL.Application.Tests.ViewModels
{
    public class BookViewModelValidationTests
    {
        [Fact] // 👈 Cambiar de [TestMethod] a [Fact]
        public void BookViewModel_Should_Require_Title()
        {
            // Arrange
            var model = new BookViewModel
            {
                Title = "", // Título vacío
                Isbn = 9788497941345,
                PublicationYear = 2024,
                Pages = 300,
                TotalCopies = 5,
                AvailableCopies = 5,
                Ubication = "Test Shelf",
                StatusId = 1
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Xunit.Assert.False(isValid); // 👈 Cambiar de Assert.IsFalse
            Xunit.Assert.True(validationResults.Any(vr => vr.MemberNames.Contains("Title")));
        }

        [Fact]
        public void BookViewModel_Should_Require_Valid_ISBN()
        {
            // Arrange
            var model = new BookViewModel
            {
                Title = "Test Book",
                Isbn = 123, // ISBN inválido
                PublicationYear = 2024,
                Pages = 300,
                TotalCopies = 5,
                AvailableCopies = 5,
                Ubication = "Test Shelf",
                StatusId = 1
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Xunit.Assert.False(isValid);
            Xunit.Assert.True(validationResults.Any(vr => vr.MemberNames.Contains("Isbn")));
        }

        [Fact]
        public void BookViewModel_Should_Require_Valid_PublicationYear()
        {
            // Arrange
            var model = new BookViewModel
            {
                Title = "Test Book",
                Isbn = 9788497941345,
                PublicationYear = 500, // Año inválido
                Pages = 300,
                TotalCopies = 5,
                AvailableCopies = 5,
                Ubication = "Test Shelf",
                StatusId = 1
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Xunit.Assert.False(isValid);
            Xunit.Assert.True(validationResults.Any(vr => vr.MemberNames.Contains("PublicationYear")));
        }

        [Fact]
        public void BookViewModel_Should_Pass_With_Valid_Data()
        {
            // Arrange
            var model = new BookViewModel
            {
                Title = "Valid Book",
                Isbn = 9788497941345,
                Description = "Test Description",
                PublicationYear = 2024,
                Pages = 300,
                TotalCopies = 5,
                AvailableCopies = 5,
                Ubication = "Test Shelf A-1",
                StatusId = 1
            };

            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            Xunit.Assert.True(isValid);
            Xunit.Assert.Empty(validationResults);
        }
    }
}