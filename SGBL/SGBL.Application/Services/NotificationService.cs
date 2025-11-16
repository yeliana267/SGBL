
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Dtos.Loan;
using SGBL.Application.Dtos.Notification;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;
using SGBL.Domain.Interfaces;

namespace SGBL.Application.Services
{
    public class NotificationService : GenericService<Notification, NotificationDto>, INotificationService
    {
        private readonly IServiceLogs _serviceLogs;
        private readonly IEmailService _emailService;


        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, IServiceLogs serviceLogs, IEmailService emailService) : base(notificationRepository, mapper, serviceLogs)
        {
            _serviceLogs = serviceLogs;
            _emailService = emailService;
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
