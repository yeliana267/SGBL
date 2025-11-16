using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public NotificationService(
            INotificationRepository notificationRepository,
            IMapper mapper,
            IServiceLogs serviceLogs,
            IEmailService emailService)
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
                    ReadDate = null
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

        public async Task<IReadOnlyList<NotificationDto>> GetRecentByUserAsync(int userId, int take = 5, bool onlyUnread = true)
        {
            if (userId <= 0)
            {
                return Array.Empty<NotificationDto>();
            }

            if (take <= 0)
            {
                take = 5;
            }

            try
            {
                var query = _notificationRepository
                    .GetAllQuery()
                    .AsNoTracking()
                    .Where(notification => notification.IdUser == userId);

                if (onlyUnread)
                {
                    query = query.Where(notification => notification.ReadDate == null);
                }

                var notifications = await query
                    .OrderByDescending(notification => notification.CreatedAt)
                    .Take(take)
                    .ToListAsync();

                return _mapper.Map<List<NotificationDto>>(notifications);
            }
            catch (Exception ex)
            {
                _serviceLogs.CreateLogWarning($"Error al obtener las notificaciones del usuario {userId}: {ex}");
                throw;
            }
        }
    }
}
