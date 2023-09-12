using Serilog;
using System.Net.Mail;

namespace ViApi.Services.EmailService;

public class EmailZaglushka : IEmailClient
{
    public async Task<bool> SendMessageAsync(string messageText, MailAddress recipient)
    {
        //Симуляция отправки сообщения
        await Task.Delay(300);
        Log.Information("Сообщение из письма: {messageText}", messageText);
        return true;
    }
}
