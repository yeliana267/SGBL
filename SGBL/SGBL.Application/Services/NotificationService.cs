using AutoMapper;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Dtos.Notification;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SGBL.Application.Services
{
    public class NotificationService : GenericService<Notification, NotificationDto>, INotificationService
    {
        private const int DefaultUnreadStatus = 1;
        private const int DefaultReadStatus = 2;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IServiceLogs _serviceLogs;
        private readonly IEmailService _emailService;
        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IServiceLogs serviceLogs, IEmailService emailService)
                : base(notificationRepository, mapper, serviceLogs)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _serviceLogs = serviceLogs;
            _emailService = emailService;
        }
        public async Task<NotificationDto?> CreateLoanNotificationAsync(int userId, int loanId, int typeId, string title, string message, bool sendEmail = false, string? email = null)
        {
            var notification = InitializeBaseNotification(userId, typeId, title, message);
            notification.IdLoan = loanId;
            notification.IdBook = null;
            return await CreateNotificationAsync(notification, sendEmail, email);
        }

        public async Task<NotificationDto?> CreateBookNotificationAsync(int userId, int bookId, int typeId, string title, string message, bool sendEmail = false, string? email = null)
        {
            var notification = InitializeBaseNotification(userId, typeId, title, message);
            notification.IdBook = bookId;
            notification.IdLoan = null;
            return await CreateNotificationAsync(notification, sendEmail, email);
        }

        public async Task<NotificationDto?> CreateUserNotificationAsync(int userId, int typeId, string title, string message, bool sendEmail = false, string? email = null)
        {
            var notification = InitializeBaseNotification(userId, typeId, title, message);
            notification.IdBook = null;
            notification.IdLoan = null;
            return await CreateNotificationAsync(notification, sendEmail, email);
        }

        public async Task<List<NotificationDto>> GetByUserAsync(int userId)
        {
            var notifications = await _notificationRepository.GetByUserAsync(userId);
            return _mapper.Map<List<NotificationDto>>(notifications);
        }

        public async Task<List<NotificationDto>> GetUnreadByUserAsync(int userId)
        {
            var notifications = await _notificationRepository.GetUnreadByUserAsync(userId, DefaultUnreadStatus);
            return _mapper.Map<List<NotificationDto>>(notifications);
        }

        public async Task<int> CountUnreadByUserAsync(int userId)
        {
            return await _notificationRepository.CountUnreadByUserAsync(userId, DefaultUnreadStatus);
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            return await _notificationRepository.MarkAsReadAsync(notificationId, DefaultReadStatus);
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId, DefaultReadStatus, DefaultUnreadStatus);
        }

        private NotificationDto InitializeBaseNotification(int userId, int typeId, string title, string message)
        {
            return new NotificationDto
            {
                IdUser = userId,
                Type = typeId,
                Title = title,
                Message = message,
                Status = DefaultUnreadStatus,
                ReadDate = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        private async Task<NotificationDto?> CreateNotificationAsync(NotificationDto notificationDto, bool sendEmail, string? email)
        {
            try
            {
                _serviceLogs.CreateLogInfo($"Creación de notificación para el usuario {notificationDto.IdUser} iniciada.");

                var entity = _mapper.Map<Notification>(notificationDto);
                entity.Status = DefaultUnreadStatus;
                entity.ReadDate = null;
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;


                var saved = await _notificationRepository.AddAsync(entity);
                var result = _mapper.Map<NotificationDto>(saved);

                if (sendEmail && !string.IsNullOrWhiteSpace(email))
                {
                    await _emailService.SendAsync(new EmailRequestDto
                    {
                        To = email,
                        Subject = notificationDto.Title,
                        HtmlBody = BuildEmailBody(notificationDto.Title, notificationDto.Message)
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error al crear notificación: {ex.Message}");
                throw;
            }
        }

        private static string BuildEmailBody(string title, string message)
        {
            var safeTitle = WebUtility.HtmlEncode(title);
            var safeMessage = WebUtility.HtmlEncode(message).Replace("\n", "<br/>");

            return $@"<h2>{safeTitle}</h2><p>{safeMessage}</p><p>Este mensaje ha sido generado automáticamente por el sistema SGBL.</p>";
        }
        public async Task SendLoanDueReminderAsync(LoanDto loan, string email, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(loan);

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("El correo electrónico del usuario es obligatorio.", nameof(email));
            }

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var subject = $"Recordatorio: el préstamo #{loan.Id} vence el {loan.DueDate:dd/MM/yyyy}";
                var messageBody = $"Tu préstamo con folio #{loan.Id} vence el {loan.DueDate:dd/MM/yyyy}.";


                var emailBody = $@"
                    <p>Hola,</p>
                    <p>Este es un recordatorio de que tu préstamo <strong>#{loan.Id}</strong> vence el <strong>{loan.DueDate:dd/MM/yyyy}</strong>.</p>
                    <p>Por favor devuelve el libro a tiempo para evitar multas.</p>
                    <p>Gracias,</p>
                    <p>Equipo de la Biblioteca</p>";

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = email,
                    Subject = subject,
                    HtmlBody = emailBody
                });

                var notification = new NotificationDto
                {
                    IdUser = loan.IdUser,
                    IdBook = loan.IdBook,
                    IdLoan = loan.Id,
                    Title = subject,
                    Message = messageBody,
                    Status = 1,
                    Type = 1,
                    ReadDate = DateTime.UtcNow
                };

                await AddAsync(notification);
                _serviceLogs.CreateLogInfo($"Recordatorio de préstamo {loan.Id} enviado a {email}.");
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error enviando recordatorio para el préstamo {loan.Id}: {ex}");
                throw;
            }
        }



    }
}
