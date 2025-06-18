using System.Net;
using System.Net.Mail;
using WeningerDemoProject.Models;
using Microsoft.Extensions.Options;
using WeningerDemoProject.Interfaces;

namespace WeningerDemoProject.Service
{
    public class SmtpEmailSender : IEmailSender, IDisposable
    {
        private readonly SmtpSettings _smtp;
        private readonly SmtpClient _client;
        private bool _disposed;
        public SmtpEmailSender(IOptions<SmtpSettings> options)
        {
            _smtp = options.Value ?? throw new ArgumentException(nameof(options), "SMTP settings are not configured");

            // Validate required settings
            if (string.IsNullOrWhiteSpace(_smtp.Host) ||
                string.IsNullOrWhiteSpace(_smtp.User) ||
                string.IsNullOrWhiteSpace(_smtp.Password) ||
                string.IsNullOrWhiteSpace(_smtp.From))
            {
                throw new ArgumentException(nameof(options), "SMTP settings must include Host, User, Password, and From address.");
            }

            _client = new SmtpClient(_smtp.Host, _smtp.Port)
            {
                 Credentials = new NetworkCredential(_smtp.User, _smtp.Password),
                 EnableSsl = true,
                 DeliveryMethod = SmtpDeliveryMethod.Network,
                 Timeout = 10000 // 10 sec
            };
        }

        public async Task SendInvitationAsync(string toEmail, string link)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email is required.", nameof(toEmail));
            if (string.IsNullOrWhiteSpace(link))
                throw new ArgumentException("Invitation link is required.", nameof(link));

            using var message = new MailMessage(_smtp.From, toEmail)
            {
                Subject = "Register Invitation",
                Body = $"Click the link to register: {link}",
                IsBodyHtml = false
            };
            await _client.SendMailAsync(message);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _client.Dispose();
                
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
