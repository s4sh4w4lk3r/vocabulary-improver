using Telegram.Bot.Types;

namespace ViApi.Services.Telegram;

public static class UpdateExtensions
{
    public static long GetId(this Update update)
    {
        if (update.Message is not null && update.Message.Chat.Id != default)
        {
            return update.Message.Chat.Id;
        }
        
        else if (update.CallbackQuery is not null && update.CallbackQuery.From.Id != default)
        {
            return update.CallbackQuery.From.Id;
        }
        else
        {
            throw new ArgumentException("Не получилось вычленить Id из Update.");
        }
    }

    public static string GetFirstname(this Update update)
    {
        if (update.Message is not null && (string.IsNullOrWhiteSpace(update.Message.Chat.FirstName) is false))
        {
            return update.Message.Chat.FirstName;
        }

        else if (update.CallbackQuery is not null && (string.IsNullOrWhiteSpace(update.CallbackQuery.From.FirstName) is false))
        {
            return update.CallbackQuery.From.FirstName;
        }
        else
        {
            throw new ArgumentException("Не получилось вычленить firstname из Update.");
        }
    }
}
