using Application.Interfaces.Setting;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;

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
            string MailServer = _config["EmailSettings:MailServer"];
            string FromEmail = _config["EmailSettings:FromEmail"];
            string Password = _config["EmailSettings:Password"];
            int Port = int.Parse(_config["EmailSettings:MailPort"]);
            var client = new SmtpClient(MailServer, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true,
            };
            MailMessage mailMessage = new MailMessage(FromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHtml,
            };
            return client.SendMailAsync(mailMessage);
        }
    }
}
