using SGBL.Application.Dtos.Email;
using SGBL.Application.Interfaces;
using SGBL.Domain.Entities;

namespace SGBL.Application.Services
{
    public class LoanNotificationService : ILoanNotificationService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IEmailService _emailService;

        // mismo valor que usas en el LoanController
        private const decimal DailyFine = 10m;

        public LoanNotificationService(
            ILoanRepository loanRepository,
            IEmailService emailService)
        {
            _loanRepository = loanRepository;
            _emailService = emailService;
        }

        public async Task ProcessDailyNotificationsAsync(CancellationToken cancellationToken = default)
        {
            var todayUtc = DateTime.UtcNow.Date;
            var threeDaysFromNowUtc = todayUtc.AddDays(3);

            var loans = await _loanRepository.GetAllWithDetailsAsync();

            // 1) 3 días antes
            var threeDaysLoans = loans
                .Where(l =>
                    l.Status == 2 &&                   // Activo
                    l.ReturnDate == null &&           // no devuelto
                    l.DueDate.Date == threeDaysFromNowUtc &&
                    !l.ReminderThreeDaysSent)
                .ToList();

            foreach (var loan in threeDaysLoans)
            {
                await SendThreeDaysReminderAsync(loan);
                loan.ReminderThreeDaysSent = true;
                loan.UpdateDate = DateTime.UtcNow;
                await _loanRepository.UpdateAsync(loan.Id, loan);
            }

            // 2) Día del vencimiento
            var dueTodayLoans = loans
                .Where(l =>
                    l.Status == 2 &&
                    l.ReturnDate == null &&
                    l.DueDate.Date == todayUtc &&
                    !l.ReminderDueDateSent)
                .ToList();

            foreach (var loan in dueTodayLoans)
            {
                await SendDueDateReminderAsync(loan);
                loan.ReminderDueDateSent = true;
                loan.UpdateDate = DateTime.UtcNow;
                await _loanRepository.UpdateAsync(loan.Id, loan);
            }

            // 3) Multa diaria
            var overdueLoans = loans
                .Where(l =>
                    l.Status == 2 &&
                    l.ReturnDate == null &&
                    l.DueDate.Date < todayUtc &&
                    (l.LastFineEmailSentUtc == null || l.LastFineEmailSentUtc.Value.Date < todayUtc))
                .ToList();

            foreach (var loan in overdueLoans)
            {
                var daysLate = (todayUtc - loan.DueDate.Date).Days;
                var fineAmount = daysLate * DailyFine;

                await SendFineEmailAsync(loan, daysLate, fineAmount);

                loan.FineAmount = fineAmount;
                loan.LastFineEmailSentUtc = DateTime.UtcNow;
                loan.UpdateDate = DateTime.UtcNow;
                await _loanRepository.UpdateAsync(loan.Id, loan);
            }
        }

        private async Task SendThreeDaysReminderAsync(Loan loan)
        {
            if (loan.User == null || string.IsNullOrEmpty(loan.User.Email))
                return;

            var body = $@"
<h3>Recordatorio de Préstamo – Faltan 3 días</h3>
<p>Hola {loan.User.Name}, tu préstamo del libro:</p>
<ul>
    <li><strong>Título:</strong> {loan.Book?.Title}</li>
    <li><strong>Fecha de préstamo:</strong> {loan.DateLoan:dd/MM/yyyy}</li>
    <li><strong>Fecha de devolución:</strong> {loan.DueDate:dd/MM/yyyy}</li>
</ul>
<p>vence en <strong>3 días</strong>. Por favor, asegúrate de devolverlo a tiempo para evitar multas.</p>
<p>¡Gracias por usar nuestro servicio de biblioteca!</p>";

            await _emailService.SendAsync(new EmailRequestDto
            {
                To = loan.User.Email,
                Subject = "Recordatorio de préstamo - Faltan 3 días",
                HtmlBody = body
            });
        }

        private async Task SendDueDateReminderAsync(Loan loan)
        {
            if (loan.User == null || string.IsNullOrEmpty(loan.User.Email))
                return;

            var body = $@"
<h3>Recordatorio de Préstamo – Vence hoy</h3>
<p>Hola {loan.User.Name}, tu préstamo del libro:</p>
<ul>
    <li><strong>Título:</strong> {loan.Book?.Title}</li>
    <li><strong>Fecha de préstamo:</strong> {loan.DateLoan:dd/MM/yyyy}</li>
    <li><strong>Fecha de devolución:</strong> {loan.DueDate:dd/MM/yyyy}</li>
</ul>
<p><strong>vence HOY</strong>. Te pedimos que devuelvas el libro lo antes posible para evitar cargos por retraso.</p>
<p>¡Gracias por usar nuestro servicio de biblioteca!</p>";

            await _emailService.SendAsync(new EmailRequestDto
            {
                To = loan.User.Email,
                Subject = "Recordatorio de préstamo - Vence hoy",
                HtmlBody = body
            });
        }

        private async Task SendFineEmailAsync(Loan loan, int daysLate, decimal fineAmount)
        {
            if (loan.User == null || string.IsNullOrEmpty(loan.User.Email))
                return;

            var body = $@"
<h3>Aviso de Multa por Retraso</h3>
<p>Hola {loan.User.Name}, el plazo de devolución del libro:</p>
<ul>
    <li><strong>Título:</strong> {loan.Book?.Title}</li>
    <li><strong>Fecha de préstamo:</strong> {loan.DateLoan:dd/MM/yyyy}</li>
    <li><strong>Fecha de devolución:</strong> {loan.DueDate:dd/MM/yyyy}</li>
</ul>
<p>ha vencido hace <strong>{daysLate}</strong> día(s).</p>
<p>La multa actual acumulada es de: <strong>{fineAmount:C}</strong>.</p>
<p>Te invitamos a devolver el libro lo antes posible para detener el aumento de la multa.</p>
<p>¡Gracias por usar nuestro servicio de biblioteca!</p>";

            await _emailService.SendAsync(new EmailRequestDto
            {
                To = loan.User.Email,
                Subject = "Multa por retraso en devolución de libro",
                HtmlBody = body
            });
        }
    }
}
