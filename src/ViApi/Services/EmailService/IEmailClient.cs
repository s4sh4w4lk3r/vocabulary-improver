using System.Net.Mail;

namespace ViApi.Services.EmailService;

public interface IEmailClient
{
    Task<bool> SendMessageAsync(string messageText, MailAddress recipient);
}
