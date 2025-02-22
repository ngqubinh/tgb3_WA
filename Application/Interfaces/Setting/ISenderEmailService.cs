namespace Application.Interfaces.Setting
{
    public interface ISenderEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml=false);
    }
}