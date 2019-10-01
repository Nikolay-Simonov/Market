using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Market.BLL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Market.BLL.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    internal class EmailSender : IEmailSender, IDisposable
    {
        private string SMTPEmail { get; }

        private SmtpClient _smtpClient { get; }

        public EmailSender(IConfiguration configuration)
        {
            string Host = configuration.GetValue<string>("SMTPSettings:Host");
            int Port = configuration.GetValue<int>("SMTPSettings:Port");
            string Password = configuration.GetValue<string>("SMTPSettings:Password");
            SMTPEmail = configuration.GetValue<string>("SMTPSettings:Email");
            _smtpClient = new SmtpClient(Host, Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(SMTPEmail, Password)
            };
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using var msg = new MailMessage(SMTPEmail, email)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            await _smtpClient.SendMailAsync(msg);
        }
    }
}