﻿using Microsoft.Extensions.Logging;
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

        public Task SendAsync(EmailRequestDto emailRequestDto)
        {
            throw new NotImplementedException();
        }

        //public async Task SendAsync(EmailRequestDto emailRequestDto)
        //{
        //    try
        //    {
        //        emailRequestDto.ToRange?.Add(emailRequestDto.To ?? "");

        //        MimeMessage email = new()
        //        {
        //            Sender = MailboxAddress.Parse(_mailSettings.EmailFrom),
        //            Subject = emailRequestDto.Subject
        //        };

        //        foreach (var toItem in emailRequestDto.ToRange ?? [])
        //        {
        //            email.To.Add(MailboxAddress.Parse(toItem));
        //        }

        //        BodyBuilder builder = new()
        //        {
        //            HtmlBody = emailRequestDto.HtmlBody
        //        };
        //        email.Body = builder.ToMessageBody();

        //        using MailKit.Net.Smtp.SmtpClient smtpClient = new();
        //        await smtpClient.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        //        await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
        //        await smtpClient.SendAsync(email);
        //        await smtpClient.DisconnectAsync(true);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An exception occured {Exception}.", ex);
        //    }
        //}
    }
}