using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using TaskManager.Helpers;
using TaskManager.Models;
using TaskManager.Interfaces;

namespace TaskManager.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value ?? throw new ArgumentNullException(nameof(smtpOptions.Value),ResponseMessages.Message.SmtpOptionNull);
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromEmail),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };
            mail.To.Add(message.ToEmail);

            using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            await smtp.SendMailAsync(mail);
        }

    }
}
