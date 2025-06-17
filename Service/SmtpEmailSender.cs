using System.Net;
using System.Net.Mail;
using WeningerDemoProject.Models;
using WeningerDemoProject.Interfaces;

namespace WeningerDemoProject.Service
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpClient _client;
        private readonly string _from;
        public SmtpEmailSender(IConfiguration config)
        {
            var smtp = SmtpConfigHelper.GetSmtpSettings(config);

            _client = new SmtpClient(smtp.Host, smtp.Port)
            {
                Credentials = new NetworkCredential(smtp.User, smtp.Password),
                EnableSsl = true
            };
            _from = smtp.From;
        }

        public async Task SendInvitationAsync(string toEmail, string link)
        {
            var message = new MailMessage(_from, toEmail)
            {
                Subject = "Register Invitation",
                Body = $"Click the link to register {link}"
            };
            await _client.SendMailAsync(message);
        }
    }

    public static class SmtpConfigHelper
    {
        public static SmtpSettings GetSmtpSettings(IConfiguration config)
        {
            return new SmtpSettings
            {
                Host = config["Smtp:Host"] ?? throw new ArgumentNullException(nameof(config), "Smtp:Host not configured"),
                Port = int.TryParse(config["Smtp:Port"], out var port)
                    ? port
                    : throw new FormatException("Smtp:Port must be a valid integer"),
                User = config["Smtp:User"] ?? throw new ArgumentNullException(nameof(config), "Smtp:User not configured"),
                Password = config["Smtp:Password"] ?? throw new ArgumentNullException(nameof(config), "Smtp:Pass not configured"),
                From = config["Smtp:From"] ?? throw new ArgumentNullException(nameof(config), "Smtp:From not configured")
            };
        }
    }
}
