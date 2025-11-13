using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Notification;
using SGBL.Application.Interfaces;
using SGBL.Application.Services;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using Assert = Xunit.Assert;

namespace SGBL.Application.Tests.Services
{
    public class NotificationServiceTests
    {
        private readonly Mock<INotificationRepository> _notificationRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServiceLogs> _serviceLogsMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _notificationRepositoryMock = new Mock<INotificationRepository>();
            _mapperMock = new Mock<IMapper>();
            _serviceLogsMock = new Mock<IServiceLogs>();
            _emailServiceMock = new Mock<IEmailService>();

            _mapperMock.Setup(m => m.Map<Notification>(It.IsAny<NotificationDto>()))
                .Returns((NotificationDto dto) => new Notification
                {
                    IdUser = dto.IdUser,
                    Type = dto.Type,
                    Title = dto.Title,
                    Message = dto.Message,
                    Status = dto.Status,
                    IdBook = dto.IdBook,
                    IdLoan = dto.IdLoan,
                    ReadDate = dto.ReadDate,
                    CreatedAt = dto.CreatedAt,
                    UpdatedAt = dto.UpdatedAt
                });

            _mapperMock.Setup(m => m.Map<NotificationDto>(It.IsAny<Notification>()))
                .Returns((Notification entity) => new NotificationDto
                {
                    Id = entity.Id,
                    IdUser = entity.IdUser,
                    Type = entity.Type,
                    Title = entity.Title,
                    Message = entity.Message,
                    Status = entity.Status,
                    IdBook = entity.IdBook,
                    IdLoan = entity.IdLoan,
                    ReadDate = entity.ReadDate,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                });

            _notificationService = new NotificationService(
                _notificationRepositoryMock.Object,
                _mapperMock.Object,
                _serviceLogsMock.Object,
                _emailServiceMock.Object);
        }

        [Fact]
        public async Task CreateLoanNotificationAsync_Should_Persist_And_Send_Email_When_Requested()
        {
            // Arrange
            var userId = 5;
            var loanId = 42;
            var typeId = 3;
            var title = "Préstamo aprobado";
            var message = "Su préstamo ha sido aprobado";
            var email = "user@example.com";

            Notification? capturedNotification = null;
            _notificationRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Notification>()))
                .ReturnsAsync((Notification notification) => notification)
                .Callback<Notification>(notification => capturedNotification = notification);

            // Act
            var result = await _notificationService.CreateLoanNotificationAsync(userId, loanId, typeId, title, message, true, email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.IdUser);
            Assert.Equal(loanId, result.IdLoan);
            Assert.Null(result.IdBook);
            Assert.Equal(typeId, result.Type);
            Assert.Equal(title, result.Title);
            Assert.Equal(message, result.Message);

            Assert.NotNull(capturedNotification);
            Assert.Equal(userId, capturedNotification!.IdUser);
            Assert.Equal(loanId, capturedNotification.IdLoan);
            Assert.Null(capturedNotification.IdBook);
            Assert.Equal(1, capturedNotification.Status);
            Assert.Null(capturedNotification.ReadDate);

            _notificationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Notification>()), Times.Once);
            _emailServiceMock.Verify(e => e.SendAsync(It.Is<EmailRequestDto>(req =>
                req.To == email &&
                req.Subject == title &&
                req.HtmlBody.Contains(message))), Times.Once);
        }
    }
}