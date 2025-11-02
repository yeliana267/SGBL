using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using SGBL.Application.Dtos.Email;
using SGBL.Application.Interfaces;
using SGBL.Domain.Settings;

namespace SGBL.Infraestructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequestDto emailRequestDto)
        {
            try
            {
                using var email = CreateEmailMessage(emailRequestDto);
                await SendEmailAsync(email);

                _logger.LogInformation("✅ Correo enviado exitosamente a: {Recipients}",
                    string.Join(", ", email.To.Select(x => x.ToString())));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando correo a: {To}", emailRequestDto.To);
                throw new ApplicationException("No se pudo enviar el correo electrónico", ex);
            }
        }

        private MimeMessage CreateEmailMessage(EmailRequestDto request)
        {
            var email = new MimeMessage();

            // Configurar remitente
            email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.EmailFrom));
            email.Sender = MailboxAddress.Parse(_mailSettings.EmailFrom);
            email.Subject = request.Subject;

            // Agregar destinatarios principales (sin duplicados)
            var allRecipients = GetValidRecipients(request.To, request.ToRange);
            foreach (var recipient in allRecipients)
            {
                email.To.Add(MailboxAddress.Parse(recipient));
            }

            // Crear cuerpo del mensaje
            var builder = new BodyBuilder
            {
                HtmlBody = request.HtmlBody
            };

            email.Body = builder.ToMessageBody();

            return email;
        }

        private static List<string> GetValidRecipients(string? single, List<string>? multiple)
        {
            var recipients = new List<string>();

            // Agregar destinatario único si es válido
            if (!string.IsNullOrWhiteSpace(single))
            {
                recipients.Add(single.Trim());
            }

            // Agregar lista de destinatarios si existe
            if (multiple != null)
            {
                recipients.AddRange(multiple
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim()));
            }

            // Eliminar duplicados
            return recipients.Distinct().ToList();
        }

        private async Task SendEmailAsync(MimeMessage email)
        {
            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

            await smtpClient.ConnectAsync(
                _mailSettings.SmtpHost,
                _mailSettings.SmtpPort,
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
        }
    }
}