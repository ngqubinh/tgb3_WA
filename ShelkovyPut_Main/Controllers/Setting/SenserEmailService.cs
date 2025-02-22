using System.Net;
using System.Net.Mail;
using Application.Interfaces.Setting;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace Infrastructure.Repositories.Setting
{
    public class SenderEmailService : ISenderEmailService
    {
        private readonly IConfiguration _config;
        public SenderEmailService(IConfiguration config)
        {
            _config = config;
        }
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            string mailServer = _config["EmailSettings:MailServer"];
            string fromEmail = _config["EmailSettings:FromEmail"];
            string password = _config["EmailSettings:Password"];
            int port = int.Parse(_config["EmailSettings:MailPort"]);

            var client = new SmtpClient(mailServer, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHtml
            };
            
            return client.SendMailAsync(mailMessage);
        }
    }
}